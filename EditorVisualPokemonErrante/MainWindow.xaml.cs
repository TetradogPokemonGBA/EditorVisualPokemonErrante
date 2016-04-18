//los derechos intelectuales de la busqueda de informacion es del usuario Razhier del post: http://wahackforo.com/t-27909/fr-e-finalizada-pokemon-erramntes
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gabriel.Cat.Extension;
using System.Drawing;
using System.IO;
using Microsoft.Win32;

namespace EditorVisualPokemonErrante
{
    public enum Estados : Int16
    {
        Dor0, Dor1, Dor2, Dor3, Dor4, Dor5, Dor6, Dor7,
        Env = 8, Que = 16, Con = 32, Par = 64, EnvG = 128
    }
    public enum VariablesEsmeralda
    {
        Special = 0x12B,
        Pokemon = 0x4F24,
        Vitalidad = 0x4F25,
        NivelYEstado = 0x4F26,
        Disponible = 0x5F29
    }
    public enum VariablesRojoFuego
    {
        Special = 0x129,
        Pokemon = 0x506C,
        Vitalidad = 0x506D,
        NivelYEstado = 0x506E,
        Disponible = 0x5071
    }
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        EstadoRadio[] estadosSleep;
        EstadoCheck[] otrosEstados;
        Pokemon[] pokemons;
        private readonly int MAXNIVEL=100;
        private readonly int MAXVIDA=65535;//por mirar de momento es el maximo con 2bytes

        public MainWindow()
        {

            string[] pokemosString = Resource1.pokedexHoenn.Split('\n');
            string[] camposPkm;
            string linea;
            Bitmap gifPkm;
            Type tipoRecuros = typeof(Resource1);
            Estados[] enumEstados = (Estados[])Enum.GetValues(typeof(Estados));
            estadosSleep = new EstadoRadio[8];
            otrosEstados = new EstadoCheck[5];
            InitializeComponent();
            this.imgPokemon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            imgIcoEsmeralda.SetImage(Resource1.Emerald);
            imgIcoRojoFuego.SetImage(Resource1.FireRed);
            for (int i = 0; i < estadosSleep.Length; i++)
            {
                estadosSleep[i] = new EstadoRadio();
                estadosSleep[i].txtNombreEstado.Text = enumEstados[i].ToString();
                estadosSleep[i].GroupName = "Sleep";
                estadosSleep[i].Tag = (Int16)enumEstados[i];
            }
            for (int i = 0, j = estadosSleep.Length; i < otrosEstados.Length; i++, j++)
            {
                otrosEstados[i] = new EstadoCheck();
                otrosEstados[i].txtNombreEstado.Text = enumEstados[j].ToString();
                otrosEstados[i].Tag = (Int16)enumEstados[j];

            }
            uniGridEstados.Children.AddRange(estadosSleep);
            uniGridEstados.Children.AddRange(otrosEstados);
            //cargo los pokemons
            pokemons = new Pokemon[pokemosString.Length-1];
            for (int i = 0; i < pokemons.Length; i++)
            {
                try
                {
                    camposPkm = pokemosString[i].Split(';');
                    camposPkm[1] = camposPkm[1].Remove(camposPkm[1].Length - 1, 1);
                    camposPkm[1] = camposPkm[1].ToLower();
                    pokemons[i] = new Pokemon(camposPkm[1], null, Convert.ToInt32(camposPkm[0]));
                    gifPkm = Resource1.GetResource(pokemons[i].NombreInternoGif);
                    pokemons[i].Img = gifPkm;

                    
                }
                catch (Exception e) { }
            }
            cmbPokemons.ItemsSource = pokemons.Ordena();
            cmbPokemons.SelectedIndex = 0;
            estadosSleep[0].IsChecked = true;

        }

        private void exportarXSE_Click(object sender, RoutedEventArgs e)
        {
            string path = GenerarNombreScript() + ".rbc";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string script = PreviewScripXSE().txtScript.Text;
            sw.Write(script);
            sw.Close();
            fs.Close();

        }

        private  string GenerarNombreScript()
        {
            return "script-PokemonErrante " + txtNombre.Text + " " + txtNumPokedex.Text.Substring(1) + "-" + DateTime.Now.Ticks;
        }

        private void exportarEVPE_Click(object sender, RoutedEventArgs e)
        {
            scriptEVPE script = new scriptEVPE(Convert.ToInt16(((Pokemon)cmbPokemons.SelectedItem).NumeroNacional), Convert.ToInt16(txtVidaQueTiene.Text), Convert.ToByte(txtNivel.Text), SumaStatus(), rbEsmeralda.IsChecked.Value);
            scriptEVPE.GetByteArray(script).Save(GenerarNombreScript() + ".evpe");
        }

        private void importarEVPE_Click(object sender, RoutedEventArgs e)
        {
            scriptEVPE script;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog().Value)
            {
                script = scriptEVPE.GetScriptEVPE(new FileStream(openFileDialog.FileName, FileMode.Open));
                txtNivel.Text = script.Nivel.ToString();
                txtVidaQueTiene.Text = script.Vida.ToString();
                cmbPokemons.SelectedIndex = script.Pokemon-1;
                rbEsmeralda.IsChecked = script.EsEsmeralda;
                CargaEstado(script.Estado);
            }

        }

        private void CargaEstado(Int16 estado)
        {
            for (int i = otrosEstados.Length-1; i >= 0; i--)
                if (estado - (Int16)otrosEstados[i].Tag >= 0)
                {
                    otrosEstados[i].IsChecked = true;
                    estado -= (Int16)otrosEstados[i].Tag;
                }
            estadosSleep[estado].IsChecked = true;
        }

        private void previsualizacionXSE_Click(object sender, RoutedEventArgs e)
        {
            PreviewScripXSE().Show();
        }

        private PrevisualizarScriptXSE PreviewScripXSE()
        {
            int nivel, vida;
            ValidarNiveYVida();
            nivel = Convert.ToInt32(txtNivel.Text);
            vida = Convert.ToInt32(txtVidaQueTiene.Text);
           return  new PrevisualizarScriptXSE(rbEsmeralda.IsChecked.Value ? imgIcoEsmeralda.Source : imgIcoRojoFuego.Source, (cmbPokemons.SelectedItem as Pokemon).NumeroNacional, vida, Convert.ToByte(nivel), SumaStatus(), rbEsmeralda.IsChecked.Value);
        }

        private void ValidarNiveYVida()
        {
            int nivel, vida;
            nivel = Convert.ToInt32(txtNivel.Text);
            vida = Convert.ToInt32(txtVidaQueTiene.Text);
            if (nivel > MAXNIVEL)
                nivel = MAXNIVEL;
            else if (nivel < 0)
                nivel = 0;
            if (vida > MAXVIDA)
                vida = MAXVIDA;
            else if (vida < 0)
                vida = 0;
            txtNivel.Text = nivel.ToString();
            txtVidaQueTiene.Text = vida.ToString();
        }

        private void previsualizacionHex_Click(object sender, RoutedEventArgs e)
        {
            int nivel, vida;
            ValidarNiveYVida();
            nivel = Convert.ToInt32(txtNivel.Text);
            vida = Convert.ToInt32(txtVidaQueTiene.Text);
            new PrevisualizarScriptHex(rbEsmeralda.IsChecked.Value ? imgIcoEsmeralda.Source : imgIcoRojoFuego.Source, (cmbPokemons.SelectedItem as Pokemon).NumeroNacional, vida, Convert.ToByte(nivel), SumaStatus(), rbEsmeralda.IsChecked.Value).Show();

        }

        private byte SumaStatus()
        {
            Int16 estadoFin = 0;
            bool encontrado = false;
            for (int i = 0; i < estadosSleep.Length && !encontrado; i++)
                if (estadosSleep[i].IsChecked.HasValue && estadosSleep[i].IsChecked.Value)
                {
                    encontrado = true;
                    estadoFin = (Int16)estadosSleep[i].Tag;
                }
            for (int i = 0; i < otrosEstados.Length; i++)
                if (otrosEstados[i].IsChecked.HasValue && otrosEstados[i].IsChecked.Value)
                    estadoFin += (Int16)otrosEstados[i].Tag;
            return Convert.ToByte(estadoFin);
        }
        private void txtVidaQueTiene_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool incorrecto = true;
            int number;
            try
            {
                number = Convert.ToInt32(e.Text);
                incorrecto = false;
            }
            catch { }
            e.Handled = incorrecto;
        }

        private void cmbPokemons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pokemon pkm = ((ComboBox)sender).SelectedItem as Pokemon;

            if (pkm != null)
            {
                imgPokemon.Image = pkm.Img;
                txtNombre.Text = pkm.Nombre;
                txtNumPokedex.Text = "#" + pkm.NumeroNacional;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ValidarNiveYVida();
            new AplicarEnLaRom(((Pokemon)cmbPokemons.SelectedItem).NumeroNacional, Convert.ToInt32(txtVidaQueTiene.Text), Convert.ToByte(txtNivel.Text), SumaStatus()).Show();
        }
    }
}

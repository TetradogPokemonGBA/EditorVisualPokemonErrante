//los derechos intelectuales de la busqueda de informacion es del usuario Razhier del post: http://wahackforo.com/t-27909/fr-e-finalizada-pokemon-erramntes
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gabriel.Cat.Extension;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
using Gabriel.Cat;

namespace EditorVisualPokemonErrante
{
    public enum Estados : short
    {
        Dormido = 0, Envenenado = 8, Quemado = 16, Congelado = 32, Paralizado = 64, Envenenamiento_grave = 128
    }
    public enum VariablesEsmeralda
    {
        Special = 0x12B,
        Pokemon = 0x4F24,
        Vitalidad = 0x4F25,
        NivelYEstado = 0x4F26,
        Disponible = 0x5F29,
        //los 3bytes para poder tener la tabla  repunteada
        RutinaOffset1 = 0x161928,
        RutinaOffset2 = 0x1619c6,
        RutinaOffset3 = 0x161a82,//uno menos
    }
    public enum VariablesRojoFuego
    {
        Special = 0x129,
        Pokemon = 0x506C,
        Vitalidad = 0x506D,
        NivelYEstado = 0x506E,
        Disponible = 0x5071,
        //los 3bytes para poder tener la tabla  repunteada
        RutinaOffset1 = 0x141d6e,
        RutinaOffset2 = 0x141df6,
        RutinaOffset3 = 0x141eae,//uno menos
    }
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly string VariableEspecialE = ((Hex)(int)VariablesEsmeralda.Special).ByteString, VariableEspecialR = ((Hex)(int)VariablesRojoFuego.Special).ByteString;
        public static readonly string VariablePokemonE = ((Hex)(int)VariablesEsmeralda.Pokemon).ByteString, VariablePokemonR = ((Hex)(int)VariablesRojoFuego.Pokemon).ByteString;
        public static readonly string VariableNivelYEstadoE = ((Hex)(int)VariablesEsmeralda.NivelYEstado).ByteString, VariableNivelYEstadoR = ((Hex)(int)VariablesRojoFuego.NivelYEstado).ByteString;
        public static readonly string VariableVitalidadE = ((Hex)(int)VariablesEsmeralda.Vitalidad).ByteString, VariableVitalidadR = ((Hex)(int)VariablesRojoFuego.Vitalidad).ByteString;
        //Busca los pointers 58 6C 46 08 en FR, o 04 1A 5D 08 en esmeralda // A00000 -> 00 00 A0 08
        static Gabriel.Cat.GBA.RomPokemon juego;
        internal static event EventHandler JuegoUpdated;
        Gabriel.Cat.Wpf.SwitchImg[] estados;
        Pokemon[] pokemons;
        private readonly int MAXNIVEL = 100;
        private readonly int MAXVIDA = 65535;//por mirar de momento es el maximo con 2bytes
        readonly char[] caracteresFinalesAdmitidosParaLocalizacion = new char[] { '0', '4', '8', 'C' };
        readonly Hex pointerDefaultFireRed = "64C685";//por mirar si es asi
        readonly Hex pointerDefaultEmerald = "D5A140";
        /*
         El último paso es decirle a la rutina la nueva longitud de la tabla. ¿Habéis visto que la tabla de FR mide 25 filas? 25 en hex es 19.
         Hay 3 bytes que indican el nº de filas de la tabla, aquí se indica la dirección donde se encuentran:

                ESMERALDA
                En 0x161928: 0x14'por defecto
                En 0x1619c6: 0x14'por defecto
                En 0x161a82: 0x13'por defecto


                FIRE RED
                En 0x141d6e: 0x19'por defecto
                En 0x141df6: 0x19'por defecto
                En 0x141eae: 0x18'por defecto

        Hay que tener en cuenta que todos los números de mapa están referidos al banco 0 en esmeralda y al banco 3 en FR. Es decir, 
        que nuestro poke sólo se podrá mover por los mapas del banco 0 en esmeralda y el banco 3 en FR. 
        Si por algún motivo quieres que aparezca en un mapa fuera de ese banco (una cueva, una casa, la zona safari...) 
        puedes ejecutar el siguiente script a la entrada del mapa:
             */
        public MainWindow()
        {
            MenuItem cargar = new MenuItem() { Header = "Cargar Juego" }, backup = new MenuItem() { Header = "Hacer BackUp" };
            string[] pokemosString = Resource1.pokedexHoenn.Split('\n');
            string[] camposPkm;
            Bitmap gifPkm;
            Type tipoRecuros = typeof(Resource1);
            Estados[] enumEstados = (Estados[])Enum.GetValues(typeof(Estados));
            estados = new Gabriel.Cat.Wpf.SwitchImg[enumEstados.Length];
            InitializeComponent();

            this.imgPokemon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            estados[0] = new Gabriel.Cat.Wpf.SwitchImg(Resource1.Dormido, Resource1.Dormido_Off) { Tag = (short)Estados.Dormido, CambiarHaciendoClick = false };
            estados[1] = new Gabriel.Cat.Wpf.SwitchImg(Resource1.Envenenado, Resource1.Envenenado_Off) { Tag = (short)Estados.Envenenado };
            estados[2] = new Gabriel.Cat.Wpf.SwitchImg(Resource1.Quemado, Resource1.Quemado_Off) { Tag = (short)Estados.Quemado };
            estados[3] = new Gabriel.Cat.Wpf.SwitchImg(Resource1.Congelado, Resource1.Congelado_Off) { Tag = (short)Estados.Congelado };
            estados[4] = new Gabriel.Cat.Wpf.SwitchImg(Resource1.Paralizado, Resource1.Paralizado_Off) { Tag = (short)Estados.Paralizado };
            estados[5] = new Gabriel.Cat.Wpf.SwitchImg(Resource1.Envenenamiento_grave, Resource1.Envenenamiento_grave_Off) { Tag = (short)Estados.Envenenamiento_grave };
            //cargo los pokemons
            pokemons = new Pokemon[pokemosString.Length - 1];
            for (int i = 0; i < pokemons.Length; i++)
            {

                camposPkm = pokemosString[i].Split(';');
                camposPkm[1] = camposPkm[1].Remove(camposPkm[1].Length - 1, 1);
                camposPkm[1] = camposPkm[1].ToLower();
                pokemons[i] = new Pokemon(camposPkm[1], null, Convert.ToInt32(camposPkm[0]));
                gifPkm = Resource1.GetResource(pokemons[i].NombreInternoGif);
                pokemons[i].Img = gifPkm;


            }
            cmbPokemons.ItemsSource = pokemons.Ordena();
            cmbPokemons.SelectedIndex = 0;
            uniGridEstados.Children.AddRange(estados.SubArray(1));
            gridImgDor.Children.Add(estados[0]);
            JuegoUpdated += (s, e) =>
            {
                if (MainWindow.Juego == null)
                {
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    imgIcoJuego.SetImage(new Bitmap(1, 1));
                }
                else if (IsEsmeralda.Value)
                {

                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                    imgIcoJuego.SetImage(Resource1.Emerald);
                }
                else
                {
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                    imgIcoJuego.SetImage(Resource1.FireRed);
                }
            };
            imgIcoJuego.MouseLeftButtonUp += (s, e) => PideJuego();
            grid.ContextMenu = new ContextMenu();
            grid.ContextMenu.Items.Add(cargar);
            grid.ContextMenu.Items.Add(backup);
            cargar.Click += (s, e) => PideJuego();
            backup.Click += (s, e) => { if (MainWindow.Juego != null) MainWindow.Juego.BackUp(); };
            PideJuego();
        }

        public static void PideSiNoEstaElJuego()
        {
            if (Juego == null)
                PideJuego();
        }

        public static void PideJuego()
        {
            Gabriel.Cat.GBA.RomPokemon romCargada;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? open = openFileDialog.ShowDialog();
            try
            {
                if (open.HasValue && open.Value)
                {
                    romCargada = new Gabriel.Cat.GBA.RomPokemon(new FileInfo(openFileDialog.FileName));
                    if (romCargada.Version != Gabriel.Cat.GBA.RomPokemon.ESMERALDA && romCargada.Version != Gabriel.Cat.GBA.RomPokemon.ROJOFUEGO)
                    {
                        MessageBox.Show("La ROM no es compatible");
                        if (MainWindow.Juego != null)
                            MessageBox.Show("ROM no cambiada!");
                    }
                    else
                    {
                        MainWindow.Juego = romCargada;

                    }
                }
                else
                {
                    //   MainWindow.Juego = null;
                    if (MainWindow.Juego == null)
                        MessageBox.Show("No se ha cargado ninguna ROM");
                    else
                        MessageBox.Show("Se queda la anterior ROM");
                }
            }
            catch { MessageBox.Show("El archivo esta ocupado por otro programa, cierrelo y vuelve a intentarlo"); }
        }

        public static Gabriel.Cat.GBA.RomPokemon Juego
        {
            get { return juego; }
            set { juego = value; if (JuegoUpdated != null) JuegoUpdated(null, null); }
        }
        public static void SaveJuego()
        {

            if (MainWindow.Juego != null && MainWindow.Juego.SePuedeModificar)
                MainWindow.Juego.Save();
            else
                MessageBox.Show("No se ha podido modificar el juego, debes de tener algun programa que lo usa!");

        }
        public static bool? IsEsmeralda
        {
            get { return Juego != null ? Juego.Version == Gabriel.Cat.GBA.RomPokemon.ESMERALDA : new bool?(); }
        }
        private bool ValidadorLocalizacionTablaRutas(string numHex)
        {
            return this.caracteresFinalesAdmitidosParaLocalizacion.Contains(numHex[numHex.Length - 1]);
        }
        public int NumeroFilasRom()
        {
            if (Juego == null)
            {
                throw new NullReferenceException();
            }
            return (Hex)Juego.ArchivoGbaPokemon[Juego.Version == Gabriel.Cat.GBA.RomPokemon.ESMERALDA ? (int)VariablesEsmeralda.RutinaOffset1 : (int)VariablesRojoFuego.RutinaOffset1];
        }

        private void exportarFRXSE_Click(object sender, RoutedEventArgs e)
        {
            ExportarXSE("FR", PreviewScripXSE().txtScriptR.Text);

        }
        private void exportarEXSE_Click(object sender, RoutedEventArgs e)
        {
            ExportarXSE("E", PreviewScripXSE().txtScriptR.Text);

        }
        private void ExportarXSE(string version, string script)
        {
            string path = GenerarNombreScript() + "-" + version + ".rbc";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(script);
            sw.Close();
            fs.Close();
        }

        private string GenerarNombreScript()
        {
            return "script-PokemonErrante " + txtNombre.Text + " " + txtNumPokedex.Text.Substring(1) + "-" + DateTime.Now.Ticks;
        }

        private void exportarEVPE_Click(object sender, RoutedEventArgs e)
        {
            scriptEVPE script = new scriptEVPE(Convert.ToInt16(((Pokemon)cmbPokemons.SelectedItem).NumeroNacional), Convert.ToInt16(txtVidaQueTiene.Text), Convert.ToByte(txtNivel.Text), SumaStatus());
            scriptEVPE.GetByteArray(script).Save(GenerarNombreScript() + ".evpep1");
        }

        private void importarEVPE_Click(object sender, RoutedEventArgs e)
        {
            scriptEVPE script;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "EVPE script part|*.evpep1";
            if (openFileDialog.ShowDialog().Value)
            {
                script = scriptEVPE.GetScriptEVPE(new FileStream(openFileDialog.FileName, FileMode.Open));
                txtNivel.Text = script.Nivel.ToString();
                txtVidaQueTiene.Text = script.Vida.ToString();
                cmbPokemons.SelectedIndex = script.Pokemon - 1;
                CargaEstado(script.Estado);
            }

        }

        private void CargaEstado(short estado)
        {
            for (int i = estados.Length - 1; i > 0; i--)
                if (estado - (short)estados[i].Tag >= 0)
                {
                    estados[i].EstadoOn = true;
                    estado -= (short)estados[i].Tag;
                }
            //me quedan los turnos sleep
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
            return new PrevisualizarScriptXSE((cmbPokemons.SelectedItem as Pokemon).NumeroNacional, vida, Convert.ToByte(nivel), SumaStatus());
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
            new PrevisualizarScriptHex((cmbPokemons.SelectedItem as Pokemon).NumeroNacional, vida, Convert.ToByte(nivel), SumaStatus()).Show();

        }

        private byte SumaStatus()
        {
            Int16 estadoFin = 0;//pongo los turnos sleep
            for (int i = 1; i < estados.Length; i++)
                if (estados[i].EstadoOn)
                    estadoFin += (Int16)estados[i].Tag;
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
            PideSiNoEstaElJuego();
            if (Juego != null)
            {
                ValidarNiveYVida();
                new AplicarEnLaRom(((Pokemon)cmbPokemons.SelectedItem).NumeroNacional, Convert.ToInt32(txtVidaQueTiene.Text), Convert.ToByte(txtNivel.Text), SumaStatus()).Show();
            }
        }
        public static void PonNumeroFilasRutinaRom(byte numFilas)
        {
            if (IsEsmeralda.HasValue)
            {
                if (IsEsmeralda.Value)
                {
                    Juego.ArchivoGbaPokemon[(long)VariablesEsmeralda.RutinaOffset1] = numFilas;
                    Juego.ArchivoGbaPokemon[(long)VariablesEsmeralda.RutinaOffset2] = numFilas;
                    Juego.ArchivoGbaPokemon[(long)VariablesEsmeralda.RutinaOffset3] = --numFilas;
                }
                else
                {
                    Juego.ArchivoGbaPokemon[(long)VariablesRojoFuego.RutinaOffset1] = numFilas;
                    Juego.ArchivoGbaPokemon[(long)VariablesRojoFuego.RutinaOffset2] = numFilas;
                    Juego.ArchivoGbaPokemon[(long)VariablesRojoFuego.RutinaOffset3] = --numFilas;
                }
            }
        }
    }
}

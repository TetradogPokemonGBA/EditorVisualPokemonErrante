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
using System.Threading;

namespace EditorVisualPokemonErrante
{
    public enum Estados : short
    {
        Dormido = 0, Envenenado = 8, Quemado = 16, Congelado = 32, Paralizado = 64, Envenenamiento_grave = 128
    }

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {//en un futuro toda la informacion se sacara de la rom!! asi es mas realista ya que son roms editadas y claro posiblemente tendran los pokemons cambiados!! por ejemplo el orden,imagen,nombre,nuevos,menos...etc..
     //en un futuro la parte de los mapas sera con los nombres y las miniaturas ;) asi es mas visual!!
     //Busca los pointers 58 6C 46 08 en FR, o 04 1A 5D 08 en esmeralda // A0 00 00 -> 00 00 A0 08
        static FrameWorkPokemonGBA.RomPokemon juego;
        internal static event EventHandler JuegoUpdated;
        Gabriel.Cat.Wpf.SwitchImg[] estados;
        private readonly int MAXNIVEL = 100;
        private readonly int MAXVIDA = 65535;//por mirar de momento es el maximo con 2bytes
        Gabriel.Cat.Wpf.SwitchImg[] imgsZ;
        private const int MAXDORMTURNS = 8;
        private bool verShiny;
        bool verTrasero = false;
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
            Estados[] enumEstados = (Estados[])Enum.GetValues(typeof(Estados));
            int selectedIndex;
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
            imgsZ = new Gabriel.Cat.Wpf.SwitchImg[MAXDORMTURNS];
            for (int i = 0; i < imgsZ.Length; i++)
            {
                imgsZ[i] = new Gabriel.Cat.Wpf.SwitchImg(Resource1.DormidoZ, Resource1.DormidoZ_Off);
                imgsZ[i].SwitchChanged += PonSleepTurn;
                imgsZ[i].Tag = i;
                gridZTruns.Children.Add(imgsZ[i]);
                Grid.SetColumn(imgsZ[i], i);
            }
            uniGridEstados.Children.AddRange(estados.SubArray(1));
            gridImgDor.Children.Add(estados[0]);
            JuegoUpdated += (s, e) =>
            {
                if (MainWindow.Juego == null)
                {
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    imgIcoJuego.SetImage(new Bitmap(1, 1));
                }
                else if (MainWindow.Juego.Version == FrameWorkPokemonGBA.RomPokemon.VersionRom.Esmeralda)
                {

                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                    imgIcoJuego.SetImage(Resource1.Emerald);
                }
                else
                {

                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                    imgIcoJuego.SetImage(Resource1.FireRed);
                    //pongo los pokemons!
                }
                if (MainWindow.Juego != null)
                {
                    MainWindow.Juego.Pokedex.CargaTodosLosPokemons();
                    cmbPokemons.ItemsSource = MainWindow.Juego.Pokedex;
                    cmbPokemons.SelectedIndex = 1;
                }
                PonRuta();
            };
            imgIcoJuego.MouseLeftButtonUp += (s, e) => PideJuego();
            grid.ContextMenu = new ContextMenu();
            grid.ContextMenu.Items.Add(cargar);
            grid.ContextMenu.Items.Add(backup);
            cargar.Click += (s, e) => PideJuego();
            backup.Click += (s, e) => { if (MainWindow.Juego != null) MainWindow.Juego.BackUp(); };
            PideJuego();
            if (MainWindow.Juego == null)
            {
                MessageBox.Show("Se necesita una rom para sacar la informacion");
                this.Close();
            }
            this.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.S)
                {
                    verShiny = !verShiny;
                    selectedIndex = cmbPokemons.SelectedIndex;
                    cmbPokemons.SelectedIndex = 0;
                    cmbPokemons.SelectedIndex = selectedIndex;
                }
                else if (e.Key == Key.T)
                {

                    verTrasero = !verTrasero;
                    selectedIndex = cmbPokemons.SelectedIndex;
                    cmbPokemons.SelectedIndex = 0;
                    cmbPokemons.SelectedIndex = selectedIndex;
                }
                else if (e.Key==Key.Up)
                {
                    if (cmbPokemons.SelectedIndex > 0)
                        cmbPokemons.SelectedIndex--;
                }
                else if (e.Key == Key.Down)
                {
                    if (cmbPokemons.SelectedIndex < cmbPokemons.Items.Count)
                        cmbPokemons.SelectedIndex++;
                }
            };
        }

        private void PonSleepTurn(object sender, bool e)
        {
            Gabriel.Cat.Wpf.SwitchImg switchImg = sender as Gabriel.Cat.Wpf.SwitchImg;
            imgsZ.WhileEach((img) => { img.EstadoOn = false; return true; });
            for (int i = 0, f = (int)switchImg.Tag; i <= f; i++)
                imgsZ[i].EstadoOn = true;
            estados[0].EstadoOn = imgsZ[1].EstadoOn;
        }

        public static void PideSiNoEstaElJuego()
        {
            if (Juego == null)
                PideJuego();
        }

        public static void PideJuego()
        {
            FrameWorkPokemonGBA.RomPokemon romCargada = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? open = openFileDialog.ShowDialog();
            try
            {
                if (open.HasValue && open.Value)
                {
                    romCargada = new FrameWorkPokemonGBA.RomPokemon(openFileDialog.FileName);
                    if (!romCargada.EsCompatiblePokemonErrante)
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

        public static FrameWorkPokemonGBA.RomPokemon Juego
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
            openFileDialog.Filter = "EVPE script part 1|*.evpep1";
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
            for (int i = 1; i < estados.Length; i++)
                estados[i].EstadoOn = false;
            for (int i = estados.Length - 1; i > 0; i--)
                if (estado - (short)estados[i].Tag >= 0)
                {
                    estados[i].EstadoOn = true;
                    estado -= (short)estados[i].Tag;
                }
            imgsZ.WhileEach((img) => { img.EstadoOn = false; return true; });
            //me quedan los turnos sleep
            for (short i = 0; i < estado + 1; i++)
                imgsZ[i].EstadoOn = true;
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
            return new PrevisualizarScriptXSE((cmbPokemons.SelectedItem as FrameWorkPokemonGBA.Pokemon).NumeroPokedexNacional, Convert.ToUInt32(vida), Convert.ToByte(nivel), SumaStatus());
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

        private byte SumaStatus()
        {
            Int16 estadoFin = 0;//pongo los turnos sleep
            for (int i = 0; i < imgsZ.Length && imgsZ[i].EstadoOn; i++)
                estadoFin = Convert.ToInt16((int)imgsZ[i].Tag);
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

            FrameWorkPokemonGBA.Pokemon pkmOri = ((ComboBox)sender).SelectedItem as FrameWorkPokemonGBA.Pokemon;
            if (pkmOri != null)
            {
                if (!verShiny)
                {
                    if (!verTrasero)
                        imgPokemon.Image = pkmOri.ImgFrontal.ToBitmap();
                    else
                        imgPokemon.Image = pkmOri.ImgTrasera.ToBitmap();
                }
                else
                {
                    if (!verTrasero)
                        imgPokemon.Image = pkmOri.ImgFrontalShiny.ToBitmap();
                    else
                        imgPokemon.Image = pkmOri.ImgTraseraShiny.ToBitmap();
                }
                txtNombre.Text = pkmOri.Nombre;
                txtNumPokedex.Text = "#" + pkmOri.NumeroPokedexNacional;
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PideSiNoEstaElJuego();
            if (Juego != null)
            {
                ValidarNiveYVida();
                new AplicarEnLaRom(((FrameWorkPokemonGBA.Pokemon)cmbPokemons.SelectedItem).NumeroPokedexNacional, Convert.ToUInt32(txtVidaQueTiene.Text), Convert.ToByte(txtNivel.Text), SumaStatus()).Show();


            }
        }

        private void btnAñadirFila_Click(object sender, RoutedEventArgs e)
        {
            FilaRuta fila;
            if (MainWindow.Juego != null)
            {
                fila = new FilaRuta();
                fila.Click += QuitarFilaClick;
                stkPanelFilasRutas.Children.Add(fila);
            }
        }

        private void cmbMapas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //pongo mapa que toca
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            PideSiNoEstaElJuego();
            if (MainWindow.Juego != null)
            {
                MainWindow.Juego.PokemonErrante.TablaRutas = FilaRuta.ToByteMatriu(stkPanelFilasRutas.Children.Casting<FilaRuta>().ToTaula());
                PonRutaInfo();
            }

        }
        private void PonRuta()
        {
            try
            {
                if (MainWindow.Juego != null)
                {
                    PonRutaInfo();
                    stkPanelFilasRutas.Children.Clear();
                    foreach (FilaRuta fila in FilaRuta.ToFilaRutaArray(MainWindow.Juego.PokemonErrante.TablaRutas))
                    {
                        fila.Click += QuitarFilaClick;
                        stkPanelFilasRutas.Children.Add(fila);
                    }
                    //pongo el cmb los mapas!!
                }
            }
            catch { }//hay problemas con las rutas...
        }

        private void QuitarFilaClick(object sender, EventArgs e)
        {
            if (ckQuitarHaciendoClick.IsChecked.Value)
                stkPanelFilasRutas.Children.Remove((FilaRuta)sender);
        }

        private void PonRutaInfo()
        {
            if (MainWindow.Juego != null)
            {
                txtOffsetTablaRutas.Text = (Hex)MainWindow.Juego.PokemonErrante.OffsetTabla;
                txtNumeroDeFilas.Text = MainWindow.Juego.PokemonErrante.NFilasRutas + "";
            }
        }

        private void btnExportarP2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImportarP2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtZTurns_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int numTurnos;
            txtVidaQueTiene_PreviewTextInput(sender, e);
            if (!e.Handled)
            {
                numTurnos = Convert.ToInt32(e.Text);
                e.Handled = numTurnos > 7 || numTurnos < 0;
                estados[0].EstadoOn = numTurnos > 0;
            }
            if (e.Text == "")
                estados[0].EstadoOn = false;
        }
    }
}

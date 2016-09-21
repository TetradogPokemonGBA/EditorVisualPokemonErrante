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
using PokemonGBAFrameWork;
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
        static RomGBA juego;
        static RomData juegoData;
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
                int total=PokemonGBAFrameWork.Pokemon.TotalPokemon(MainWindow.Juego);
                if (MainWindow.Juego == null)
                {
                    Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    imgIcoJuego.SetImage(new Bitmap(1, 1));
                }
                else if (Edicion.GetEdicion(MainWindow.Juego).AbreviacionRom.Equals(Edicion.ABREVIACIONESMERALDA))
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
                    if(JuegoData.Pokedex[0].OrdenPokedexNacional==151)
                       JuegoData.Pokedex[0].OrdenPokedexNacional = 0;//es misigno que lo detecta como MEW
                    PokemonGBAFrameWork.Pokemon.Orden = PokemonGBAFrameWork.Pokemon.OrdenPokemon.Nacional;
                    JuegoData.Pokedex.Ordena();
                    cmbPokemons.ItemsSource =PokemonGBAFrameWork.Pokemon.FiltroSinNoPokes(JuegoData.Pokedex); 
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
                
                    switch (e.Key)
                    {
                        case Key.S:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            verShiny = !verShiny;
                            selectedIndex = cmbPokemons.SelectedIndex;
                            cmbPokemons.SelectedIndex = 0;
                            cmbPokemons.SelectedIndex = selectedIndex;
                        }
                            break;
                        case Key.T:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            verTrasero = !verTrasero;
                            selectedIndex = cmbPokemons.SelectedIndex;
                            cmbPokemons.SelectedIndex = 0;
                            cmbPokemons.SelectedIndex = selectedIndex;
                        }
                            break;
                        case Key.Up:
                            if (cmbPokemons.SelectedIndex > 0)
                                cmbPokemons.SelectedIndex--;
                            break;
                        case Key.Down:
                            if (cmbPokemons.SelectedIndex < cmbPokemons.Items.Count)
                                cmbPokemons.SelectedIndex++;
                            break;
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
            RomGBA romCargada = null;
            PokemonErrante.Ruta[] rutasPokemonErrante;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? open = openFileDialog.ShowDialog();
            bool compatible;
            try
            {
                if (open.HasValue && open.Value)
                {
                    romCargada = new RomGBA(new FileInfo(openFileDialog.FileName));
                  
                    try
                    {
                        rutasPokemonErrante=PokemonErrante.Ruta.GetRutas(romCargada, Edicion.GetEdicion(romCargada), CompilacionRom.GetCompilacion(romCargada));
                        //pongo las rutas :D
                        compatible = true;
                    }
                    catch { compatible = false; }
                    if (!compatible)
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

        public static RomGBA Juego
        {
            get { return juego; }
            set {
                if (value == null) throw new ArgumentNullException();
                  juego = value;
                  JuegoData = RomData.GetRomData(juego);
                if (JuegoUpdated != null) JuegoUpdated(null, null);
            }
        }

        public static RomData JuegoData
        {
            get
            {
                return juegoData;
            }

           private set
            {
                juegoData = value;
            }
        }

        public static void SaveJuego()
        {

            if (MainWindow.Juego != null&&MainWindow.Juego.SePuedeModificar)
                MainWindow.Juego.Guardar();
            else
                MessageBox.Show("No se ha podido modificar el juego, debes de tener algun programa que lo usa!");

        }

        private void exportarFRXSE_Click(object sender, RoutedEventArgs e)
        {
            ExportarXSE(PreviewScripXSE().txtScript.Text);

        }

        private void ExportarXSE(string script)
        {
            string path = GenerarNombreScript() + "-" + JuegoData.Edicion.AbreviacionRom+JuegoData.Edicion.InicialIdioma + ".rbc";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(script);
            sw.Close();
            fs.Close();
        }

        private string GenerarNombreScript()
        {
            string nombre = txtNombre.Text;
            if (txtNumPokedex.Text == "#0" && nombre.Contains("?"))
                nombre = "Missigno";
            return "script-PokemonErrante " + nombre + " " + txtNumPokedex.Text.Substring(1) + "-" + DateTime.Now.Ticks;
        }

        private void exportarEVPE_Click(object sender, RoutedEventArgs e)
        {
            scriptEVPE script = new scriptEVPE(Convert.ToInt16(((PokemonGBAFrameWork.Pokemon)cmbPokemons.SelectedItem).OrdenPokedexNacional+1), Convert.ToInt16(txtVidaQueTiene.Text), Convert.ToByte(txtNivel.Text), SumaStatus());
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
            return new PrevisualizarScriptXSE(new PokemonErrante.Pokemon((cmbPokemons.SelectedItem as PokemonGBAFrameWork.Pokemon), vida, Convert.ToByte(nivel), SumaStatus()));
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

        private void txtVidaQueTiene_PreviewTextInput(object sender, KeyboardEventArgs e)
        {
            bool incorrecto = true;
            int number;
            try
            {
                number = Convert.ToInt32(((TextBox)sender).Text);
                if (number < Convert.ToInt32(txblVidaTotalEspecie.Text))
                    incorrecto = false;
                else if (number > MAXVIDA)
                    txtVidaQueTiene.Text = MAXVIDA + "";
            }
            catch { txtVidaQueTiene.Text = MAXVIDA + ""; }
            e.Handled = incorrecto;
        }

        private void cmbPokemons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            PokemonGBAFrameWork.Pokemon pkmOri = ((ComboBox)sender).SelectedItem as PokemonGBAFrameWork.Pokemon;
            if (pkmOri != null)
            {
                if (!verShiny)
                {
                    if (!verTrasero)
                        imgPokemon.Image = pkmOri.Sprites.ImagenFrontalNormal;
                    else
                        imgPokemon.Image = pkmOri.Sprites.ImagenTraseraNormal;
                }
                else
                {
                    if (!verTrasero)
                        imgPokemon.Image = pkmOri.Sprites.ImagenFrontalShiny;
                    else
                        imgPokemon.Image = pkmOri.Sprites.ImagenTraseraShiny;
                }
                txtNombre.Text = pkmOri.Nombre;
                txtNumPokedex.Text = "#" + pkmOri.OrdenPokedexNacional;
                txblVidaTotalEspecie.Text = pkmOri.HpMaxima(Convert.ToByte(txtNivel.Text)) + "";
                txtVidaQueTiene.Text = txblVidaTotalEspecie.Text;
            }
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PideSiNoEstaElJuego();
            if (Juego != null)
            {
                ValidarNiveYVida();
                new AplicarEnLaRom(((PokemonGBAFrameWork.Pokemon)cmbPokemons.SelectedItem), Convert.ToInt32(txtVidaQueTiene.Text), Convert.ToByte(txtNivel.Text), SumaStatus()).Show();


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
              //  MainWindow.Juego.PokemonErrante.TablaRutas = FilaRuta.ToByteMatriu(stkPanelFilasRutas.Children.Casting<FilaRuta>().ToTaula());
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
              /*      foreach (FilaRuta fila in FilaRuta.ToFilaRutaArray(MainWindow.Juego.PokemonErrante.TablaRutas))
                    {
                        fila.Click += QuitarFilaClick;
                        stkPanelFilasRutas.Children.Add(fila);
                    }*/
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
              //  txtOffsetTablaRutas.Text = (Hex)MainWindow.Juego.PokemonErrante.OffsetTabla;
               // txtNumeroDeFilas.Text = MainWindow.Juego.PokemonErrante.NFilasRutas + "";
            }
        }

        private void btnExportarP2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImportarP2_Click(object sender, RoutedEventArgs e)
        {

        }


        private void txtNivel_PreviewTextInput(object sender, KeyboardEventArgs e)
        {
            const byte NIVELLMAX = 100,NIVELLMIN=1;
            int nivell = NIVELLMIN;
            try
            {
                nivell = Convert.ToInt32(txtNivel.Text);
            }
            catch { }
            if (nivell < NIVELLMIN)
            {
                nivell = NIVELLMIN;
            }

            else if (nivell > NIVELLMAX)
            {
                nivell = NIVELLMAX;
            }
            txtNivel.Text = nivell + "";

            txblVidaTotalEspecie.Text = (cmbPokemons.SelectedItem as PokemonGBAFrameWork.Pokemon).HpMaxima(Convert.ToByte(nivell)) + "";
            if (Convert.ToInt32(txtVidaQueTiene.Text) > Convert.ToInt32(txblVidaTotalEspecie.Text))
                txtVidaQueTiene.Text = txblVidaTotalEspecie.Text;



        }
    }
}

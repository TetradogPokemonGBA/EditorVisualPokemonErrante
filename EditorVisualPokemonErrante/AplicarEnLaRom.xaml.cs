using Gabriel.Cat;
using Gabriel.Cat.Extension;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Gabriel.Cat.Binaris;
using System.Drawing;
using System.Windows.Media.Effects;

namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para AplicarEnLaRom.xaml
    /// </summary>
    public partial class AplicarEnLaRom : Window
    {
        int pokemon, vida;
        byte nivel, stat;
        const int LENGTHSCRIPT = 19;
        const long BYTESMEMORIA = 16;
        private const int INICIO = 0x800000;
        private const int BYTEEMPTY = 0xFF;
        byte[] bytesScript;

        public AplicarEnLaRom(int pokemon, int vida, byte nivel, byte stat)
        {
            
            MenuItem cargar = new MenuItem() { Header = "Cargar" }, backup = new MenuItem() { Header = "Hacer BackUp" },quitarScript=new MenuItem() { Header="Quitar script"};
            uint direccion;
            this.pokemon = pokemon;
            this.vida = vida;
            this.nivel = nivel;
            this.stat = stat;
            InitializeComponent();
            this.stkPanel.MouseLeftButtonDown += (s, e) => { try { this.DragMove(); } catch { } };
            PonSiEstaElJuego();
            MainWindow.JuegoUpdated += (s, e) => PonSiEstaElJuego();
            grid.ContextMenu = new ContextMenu();
            grid.ContextMenu.Items.Add(cargar);
            grid.ContextMenu.Items.Add(backup);
            grid.ContextMenu.Items.Add(quitarScript);
            cargar.Click += (s, e) =>MainWindow.PideJuego();
            backup.Click += (s, e) => { if (MainWindow.Juego != null) MainWindow.Juego.BackUp(); };
            quitarScript.Click += (s, e) => {
                if (MainWindow.Juego != null)
                {
                    direccion =Convert.ToUInt32(MainWindow.Juego.ArchivoGbaPokemon.IndexOf(bytesScript));
                    if (direccion>0)
                    {
                        MainWindow.Juego.QuitarBytes(direccion, LENGTHSCRIPT);
                        MainWindow.Juego.Save();
                        txtOffset.Text = "";
                    }else { MessageBox.Show("El script no esta en la rom"); }
                }
                    };

        }

        private void btnAplicarEnJuego_Click(object sender, RoutedEventArgs e)
        {
            int direccion;
            if (MainWindow.Juego == null)
            {
                MessageBox.Show("primero carga la ROM");
            }
            else
            {
                direccion = MainWindow.Juego.DireccionBytes(0,bytesScript);
                if (direccion < 0)
                {
                    PonDireccionCorrecta();
                    PonBytes();
                    
                }
                else
                {
                    txtOffset.Text = (Hex)direccion;
                    MessageBox.Show("El scritp ya esta en la ROM!");
                }
            }

        }

        private void PonDireccionCorrecta()
        {
           if(!ValidaLugarDondeSePondra())
                txtOffset.Text = (Hex)MainWindow.Juego.DireccionEspacio(LENGTHSCRIPT, INICIO);
        }

        private void PonBytes()
        {
            if (MainWindow.Juego.SePuedeModificar)
            {
                MainWindow.Juego.PonBytes((Hex)txtOffset.Text, bytesScript);
                MainWindow.SaveJuego();
            }
            else
            {
                txtOffset.Text = "";
                MessageBox.Show("No se puede modificar la ROM, mira si hay algun programa que lo use y cirralo");
            }
  
        }

        private bool ValidaLugarDondeSePondra()
        {
            bool valida;
            EliminarEspacios();
            if (txtOffset.Text != "" && Hex.ValidaString(txtOffset.Text))
            {
                valida = MainWindow.Juego.ValidaDireccion((uint)(Hex)txtByteScript.Text,Convert.ToUInt32(bytesScript.Length));
            }
            else valida = false;
            return valida;
        }

        private void EliminarEspacios()
        {
            txtOffset.Text = txtOffset.Text.TrimStart(' ').TrimEnd(' ');
        }




        private void PonImagen()
        {
            if (MainWindow.Juego!=null)
            {
                if (MainWindow.Juego.Version==FrameWorkPokemonGBA.RomPokemon.Juego.Esmeralda)
                {
                    imgVersionGame.SetImage(Resource1.Emerald);
                    grid.Background = new SolidColorBrush(Colors.Black);
                    Background = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    imgVersionGame.SetImage(Resource1.FireRed);
                    grid.Background = new SolidColorBrush(Colors.Red);
                    Background = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PonSiEstaElJuego()
        {
            int direccion;
            if (MainWindow.Juego != null)
            {
                txtByteScript.Text = MainWindow.Juego.PokemonErrante.BytesScriptString(pokemon, vida, nivel, stat);
                bytesScript= MainWindow.Juego.PokemonErrante.BytesScript(pokemon, vida, nivel, stat);
                direccion = MainWindow.Juego.ArchivoGbaPokemon.IndexOf(bytesScript);
                if (direccion > 0)
                    txtOffset.Text = (Hex)direccion;
                PonImagen();
            }
            else
            {
                grid.Background = new SolidColorBrush(Colors.Gray);
                imgVersionGame.SetImage(new Bitmap(1, 1));
            }
        }


    }
}

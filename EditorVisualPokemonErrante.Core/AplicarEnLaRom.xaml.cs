
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

using System.Drawing;
using System.Windows.Media.Effects;

using PokemonGBAFramework.Core;
using Gabriel.Cat.S.Utilitats;
using Gabriel.Cat.S.Extension;

namespace EditorVisualPokemonErrante.Core
{
    /// <summary>
    /// Lógica de interacción para AplicarEnLaRom.xaml
    /// </summary>
    public partial class AplicarEnLaRom : Window
    {
        PokemonErrante.Pokemon pokemon;
        const int LENGTHSCRIPT = 19;
        const long BYTESMEMORIA = 16;
        private const int INICIO = 0x800000;
        private const int BYTEEMPTY = 0xFF;
        byte[] bytesScript;

        public AplicarEnLaRom(Pokemon pokemon, int vida, byte nivel, byte stat)
        {
            
            MenuItem cargar = new MenuItem() { Header = "Cargar" }, backup = new MenuItem() { Header = "Hacer BackUp" };
            this.pokemon = new PokemonErrante.Pokemon() {Errante= pokemon,Vida= new Word((ushort)vida),Nivel= nivel,Stats= stat };
            InitializeComponent();
            this.stkPanel.MouseLeftButtonDown += (s, e) => { try { this.DragMove(); } catch { } };
            PonSiEstaElJuego();
            MainWindow.JuegoUpdated += (s, e) => PonSiEstaElJuego();
            grid.ContextMenu = new ContextMenu();
            grid.ContextMenu.Items.Add(cargar);
            grid.ContextMenu.Items.Add(backup);
            cargar.Click += (s, e) =>  MainWindow.PideJuego();
         //   backup.Click += (s, e) => { if (MainWindow.Juego != null) MainWindow.Juego.BackUp(); };


        }

        private void btnAplicarEnJuego_Click(object sender, RoutedEventArgs e)
        {
            Hex direccion;
            if (MainWindow.Juego == null)
            {
                MessageBox.Show("primero carga la ROM");
            }
            else
            {
                direccion = MainWindow.Juego.Data.SearchArray(bytesScript);
                if (btnBuscarEspacioLibre.Content.ToString()=="Aplicar")
                {
                    if(direccion>-1)
                       txtOffset.Text = direccion;
                    else
                       PonDireccionCorrecta();
                    PonBytes();
                    btnBuscarEspacioLibre.Content = "Quitar";
                    
                }
                else
                {
                    MainWindow.Juego.Data.Remove((int) direccion, LENGTHSCRIPT);
                    txtOffset.Text = "";
                    btnBuscarEspacioLibre.Content = "Aplicar";
                }
            }

        }

        private void PonDireccionCorrecta()
        {
           if(!ValidaLugarDondeSePondra())
                txtOffset.Text = (Hex)MainWindow.Juego.Data.SearchEmptyBytes(INICIO,LENGTHSCRIPT);
        }

        private void PonBytes()
        {
            //if (MainWindow.Juego.SePuedeModificar)
            //{
            //    PokemonGBAFrameWork.PokemonErrante.Pokemon.SetPokemonScript(MainWindow.Juego,MainWindow.RomData.Edicion,MainWindow.RomData.Compilacion,(Hex)txtOffset.Text, pokemon);
            //    MainWindow.SaveJuego();
            //}
            //else
            //{
                
            //    MessageBox.Show("No se puede modificar la ROM, mira si hay algun programa que lo use y cirralo");
            //}
  
        }

        private bool ValidaLugarDondeSePondra()
        {
            bool valida;
            EliminarEspacios();
            if (txtOffset.Text != "" && Hex.ValidaString(txtOffset.Text))
            {

                //valida = PokemonErrante.(MainWindow.Juego,(Hex)txtOffset.Text,bytesScript.Length);
                valida = true;
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
                if (MainWindow.Juego.Edicion.EsEsmeralda)
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
            //Hex direccion;
            //if (MainWindow.Juego != null)
            //{
            //    txtByteScript.Text = PokemonErrante.Pokemon.BytesScriptString(MainWindow.Juego,MainWindow.RomData.Edicion,MainWindow.RomData.Compilacion,pokemon);
            //    bytesScript = PokemonErrante.Pokemon.BytesScript(MainWindow.Juego, MainWindow.RomData.Edicion, MainWindow.RomData.Compilacion, pokemon);
            //    direccion = BloqueBytes.SearchBytes(MainWindow.Juego,bytesScript);
            //    if (direccion > 0)
            //    {
            //        txtOffset.Text = direccion;
            //        btnBuscarEspacioLibre.Content = "Quitar";
            //    }
            //    else {
            //        txtOffset.Text = "";
            //        btnBuscarEspacioLibre.Content = "Aplicar";
            //    }
            //    PonImagen();
            //}
            //else
            //{
            //    grid.Background = new SolidColorBrush(Colors.Gray);
            //    imgVersionGame.SetImage(new Bitmap(1, 1));
            //}
        }


    }
}

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
namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para AplicarEnLaRom.xaml
    /// </summary>
    public partial class AplicarEnLaRom : Window
    {
        bool isEsmeralda;
        int pokemon, vida;
        byte nivel, stat;
        const long BYTESMEMORIA = 16;
        private const int INICIO = 0x800000;
        private const int BYTEEMPTY = 0xFF;
        List<Hex> camposScript;
        public AplicarEnLaRom(int pokemon, int vida, byte nivel, byte stat)
        {
            camposScript = new List<Hex>();
            this.pokemon = pokemon;
            this.vida = vida;
            this.nivel = nivel;
            this.stat = stat;
            InitializeComponent();
            PonSiEstaElJuego();
        }

        private void btnAplicarEnJuego_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytesScript;
            EliminarEspacios();
            try
            {
                if (MainWindow.Juego == null)
                {
                    MessageBox.Show("primero carga la ROM");
                }
                else if (ValidaLugarDondeSePondra())
                {
                    bytesScript = GetBytes();
                    for (int i = (Hex)txtOffset.Text, f = i + bytesScript.Length, pos = 0; i < f; i++, pos++)
                        MainWindow.Juego.ArchivoGbaPokemon[i] = bytesScript[pos];
                    MainWindow.Juego.Save();
                    //  MessageBox.Show("Acabada de aplicar los cambios en la direccion puesta");
                }
                else
                {
                    MessageBox.Show("El lugar especificado no esta libre('0xFF')");
                }
            }
            catch {
                MessageBox.Show("el la direccion del offset tiene que estar en hexadecimal");
            }

        }

        private bool ValidaLugarDondeSePondra()
        {
            EliminarEspacios();
            return Gabriel.Cat.Binaris.BuscarBloques.BuscaBloque(MainWindow.Juego.ArchivoGbaPokemon, BYTESMEMORIA, (Hex)txtOffset.Text, BYTEEMPTY).Equals((Hex)txtOffset.Text);
        }

        private void EliminarEspacios()
        {
            txtOffset.Text = txtOffset.Text.TrimStart(' ').TrimEnd(' ');
        }

        private byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < camposScript.Count; i++)
                bytes.AddRange(Serializar.GetBytes((short)camposScript[i]));
            return bytes.Filtra((byteAMirar) => { return byteAMirar != 0x00; }).ToArray();
        }

        private void btnCargarJuego_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog().Value)
                {
                    MainWindow.Juego = new Gabriel.Cat.GBA.RomPokemon(new FileInfo(openFileDialog.FileName));
                    if (MainWindow.Juego.Version != Gabriel.Cat.GBA.RomPokemon.ZAFIRO)
                {
                    isEsmeralda = MainWindow.Juego.Version != Gabriel.Cat.GBA.RomPokemon.ROJOFUEGO;
                    
                }
                else
                    {
                        MainWindow.Juego = null;
                        txtNombreArchivo.Text = "";
                        MessageBox.Show("La ROM no es compatible");
                    }
                    }
                else
                {
                    txtNombreArchivo.Text = "";
                    MainWindow.Juego = null;
                    MessageBox.Show("No se ha cargado ninguna ROM");
                }

                PonSiEstaElJuego();
        }

        private void PonImagen()
        {
            if (isEsmeralda)
            {
                imgVersionGame.SetImage(Resource1.Emerald);
            }
            else
            {
                imgVersionGame.SetImage(Resource1.FireRed);
            }
        }

        private void PonSiEstaElJuego()
        {
            if (MainWindow.Juego != null)
            {
                camposScript.Clear();
                txtHexScript.Text = new PrevisualizarScriptHex(pokemon, vida, nivel, stat).txtScriptR.Text;
                txtNombreArchivo.Text = MainWindow.Juego.NombreHack;
                txtOffset.Text = "";
                foreach (string hexNumString in txtHexScript.Text.Replace("\r\n", "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    camposScript.Add(hexNumString.Split('x')[1]);
                }
                PonImagen();
            }
        }

        private void btnBuscarEspacioLibre_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Juego == null)
            {
                MessageBox.Show("primero carga la ROM");
            }
            else
            {
                txtOffset.Text = (Hex)Gabriel.Cat.Binaris.BuscarBloques.BuscaBloque(MainWindow.Juego.ArchivoGbaPokemon, BYTESMEMORIA, INICIO, BYTEEMPTY);
            }

        }
    }
}

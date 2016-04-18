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
        Gabriel.Cat.GBA.RomPokemon juego;
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
            juego = null;
            this.pokemon = pokemon;
            this.vida = vida;
            this.nivel = nivel;
            this.stat = stat;
            InitializeComponent();
        }

        private void btnAplicarEnJuego_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytesScript;
            if (juego == null)
            {
                MessageBox.Show("primero carga la ROM");
            }
            else if (ValidaLugarDondeSePondra())
            {
                bytesScript = GetBytes();
                for (int i= (Hex)lugarHaPonerLosBytes.Text,f=i+bytesScript.Length,pos=0;i<f;i++,pos++)
                     juego.ArchivoGbaPokemon[i]=bytesScript[pos];
                juego.Save();
              //  MessageBox.Show("Acabada de aplicar los cambios en la direccion puesta");
            }
            else
            {
                MessageBox.Show("El lugar especificado no esta libre('0xFF')");
            }

        }

        private bool ValidaLugarDondeSePondra()
        {
            return Gabriel.Cat.Binaris.BuscarBloques.BuscaBloque(juego.ArchivoGbaPokemon, BYTESMEMORIA, (Hex)lugarHaPonerLosBytes.Text, BYTEEMPTY).Equals((Hex)lugarHaPonerLosBytes.Text);
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
            try
            {
                if (openFileDialog.ShowDialog().Value)
                {
                    juego = new Gabriel.Cat.GBA.RomPokemon(new FileInfo(openFileDialog.FileName));
                    isEsmeralda = juego.Version != Gabriel.Cat.GBA.RomPokemon.ROJOFUEGO;
                    if (isEsmeralda)
                    {
                        if (juego.Version != Gabriel.Cat.GBA.RomPokemon.ESMERALDA)
                            throw new Exception();
                        imgVersionGame.SetImage(Resource1.Emerald);
                    }
                    else
                    {
                        imgVersionGame.SetImage(Resource1.FireRed);
                    }
                }
                else
                {
                    nombreArchivo.Text = "";
                    juego = null;
                    MessageBox.Show("No se ha cargado ninguna ROM");
                }
                if (juego != null)
                {
                    camposScript.Clear();
                    txtHexScript.Text = new PrevisualizarScriptHex(null, pokemon, vida, nivel, stat, isEsmeralda).txtScript.Text;
                    nombreArchivo.Text = juego.NombreHack;
                    lugarHaPonerLosBytes.Text = "";
                    foreach (string hexNumString in txtHexScript.Text.Replace("\r\n", "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        camposScript.Add(hexNumString.Split('x')[1]);
                    }
                  //  MessageBox.Show("ROM cargada correctamente!");
                }
            }
            catch {
                juego = null;
                nombreArchivo.Text = "";
                MessageBox.Show("La ROM no es compatible");
                
            }

        }

        private void btnBuscarEspacioLibre_Click(object sender, RoutedEventArgs e)
        {
            if (juego == null)
            {
                MessageBox.Show("primero carga la ROM");
            }
            else
            {
                lugarHaPonerLosBytes.Text = (Hex)Gabriel.Cat.Binaris.BuscarBloques.BuscaBloque(juego.ArchivoGbaPokemon, BYTESMEMORIA, INICIO, BYTEEMPTY);
            }

        }
    }
}

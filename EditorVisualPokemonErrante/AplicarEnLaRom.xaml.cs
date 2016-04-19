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
        int pokemon, vida;
        byte nivel, stat;
        const long BYTESMEMORIA = 16;
        private const int INICIO = 0x800000;
        private const int BYTEEMPTY = 0xFF;
        byte[] bytesScript;
       

        public AplicarEnLaRom(int pokemon, int vida, byte nivel, byte stat)
        {

            this.pokemon = pokemon;
            this.vida = vida;
            this.nivel = nivel;
            this.stat = stat;
            InitializeComponent();
            PonSiEstaElJuego();
            MainWindow.JuegoUpdated += (s, e) => PonSiEstaElJuego();
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
                direccion = Array.IndexOf(MainWindow.Juego.ArchivoGbaPokemon,bytesScript);
                if (direccion < 0)
                {
                    try
                    {
                        if (txtOffset.Text == "")
                            throw new Exception();
                        PonBytes();
                    }
                    catch
                    {
                        btnBuscarEspacioLibre_Click(null, null);
                        PonBytes();
                    }
                }
                else
                {
                    txtOffset.Text = (Hex)direccion;
                    MessageBox.Show("El scritp ja esta en la ROM!");
                }
            }

        }

        private void PonBytes()
        {
            if (ValidaLugarDondeSePondra())
            {
                for (int i = (Hex)txtOffset.Text, f = i + bytesScript.Length, pos = 0; i < f; i++, pos++)
                    MainWindow.Juego.ArchivoGbaPokemon[i] = bytesScript[pos];
                MainWindow.Juego.Save();
            }
            else
            {
                MessageBox.Show("El lugar especificado no esta libre('0xFF')");
            }
        }

        public static byte[] GetBytes(string scriptByte)
        {
            List<byte> bytes = new List<byte>();
            string[] bytesString = scriptByte.Replace("\r\n", " ").Split(' ');
            for (int i = 0; i < bytesString.Length; i++)
                bytes.Add(Convert.ToByte((int)(Hex)bytesString[i]));
            return bytes.ToArray();
        }

        private bool ValidaLugarDondeSePondra()
        {
            bool valida;
            Hex offSet;
            EliminarEspacios();
            if (txtOffset.Text != "" && Hex.ValidaString(txtOffset.Text))
            {
                offSet = (Hex)txtOffset.Text;
                valida = offSet + bytesScript.Length <= MainWindow.Juego.ArchivoGbaPokemon.Length;
                for (int i = offSet,j=0; i < MainWindow.Juego.ArchivoGbaPokemon.Length && valida&&j<bytesScript.Length; i++,j++)
                    valida = MainWindow.Juego.ArchivoGbaPokemon[i] == BYTEEMPTY;
            }
            else valida = false;
            return valida;
        }

        private void EliminarEspacios()
        {
            txtOffset.Text = txtOffset.Text.TrimStart(' ').TrimEnd(' ');
        }



        private void btnCargarJuego_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? open = openFileDialog.ShowDialog();
            if (open.HasValue && open.Value)
            {
                MainWindow.Juego = new Gabriel.Cat.GBA.RomPokemon(new FileInfo(openFileDialog.FileName));
                if (MainWindow.Juego.Version == Gabriel.Cat.GBA.RomPokemon.ZAFIRO)
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
            if (MainWindow.IsEsmeralda.HasValue)
            {
                if (MainWindow.IsEsmeralda.Value)
                {
                    imgVersionGame.SetImage(Resource1.Emerald);
                    grid.Background = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    imgVersionGame.SetImage(Resource1.FireRed);
                    grid.Background = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void PonSiEstaElJuego()
        {

            if (MainWindow.Juego != null)
            {
                txtByteScript.Text = AplicarEnLaRom.BytesScript(MainWindow.IsEsmeralda.Value, pokemon, vida, nivel, stat);
                txtNombreArchivo.Text = MainWindow.Juego.NombreHack;
                txtOffset.Text = "";

                bytesScript = GetBytes(txtByteScript.Text);
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
        public static string BytesScript(bool isEsmeralda, int pokemon, int vida, byte nivel, byte stat)
        {
            string script, variableQueToca, stringQueToca;
            stringQueToca = isEsmeralda ? MainWindow.VariableEspecialE.Split('x')[1] : MainWindow.VariableEspecialR.Split('x')[1];
            stringQueToca = stringQueToca.PadLeft(4, '0');
            script = "25 " + stringQueToca.Substring(0, 2) + " " + stringQueToca.Substring(2, 2);
            stringQueToca = isEsmeralda ? MainWindow.VariablePokemonE.Split('x')[1] : MainWindow.VariablePokemonR.Split('x')[1];
            variableQueToca = ((Hex)pokemon).Number.PadLeft(4, '0');
            script += "\r\n16 " + stringQueToca.Substring(0, 2) + " " + stringQueToca.Substring(2, 2) + " " + variableQueToca.Substring(0, 2) + " " + variableQueToca.Substring(2, 2);
            stringQueToca = isEsmeralda ? MainWindow.VariableVitalidadE.Split('x')[1] : MainWindow.VariableVitalidadE.Split('x')[1];
            variableQueToca = ((Hex)vida).Number.PadLeft(4, '0');
            script += "\r\n16 " + stringQueToca.Substring(0, 2) + " " + stringQueToca.Substring(2, 2) + " " + variableQueToca.Substring(0, 2) + " " + variableQueToca.Substring(2, 2);
            stringQueToca = isEsmeralda ? MainWindow.VariableNivelYEstadoE.Split('x')[1] : MainWindow.VariableNivelYEstadoR.Split('x')[1];
            script += "\r\n16 " + stringQueToca.Substring(0, 2) + " " + stringQueToca.Substring(2, 2) + " " + ((Hex)stat).Number.PadLeft(2, '0') + " " + ((Hex)nivel).Number.PadLeft(2, '0');
            script += "\r\nFF";
            return script;
        }
    }
}

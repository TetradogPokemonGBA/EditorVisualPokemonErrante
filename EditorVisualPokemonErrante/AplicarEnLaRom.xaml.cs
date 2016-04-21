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
        private static readonly byte[] bytesEmpty;
         

        static AplicarEnLaRom()
        {
            bytesEmpty = new byte[LENGTHSCRIPT];
            for (int i = 0; i < bytesEmpty.Length; i++)
                bytesEmpty[i] = BYTEEMPTY;
        }
        public AplicarEnLaRom(int pokemon, int vida, byte nivel, byte stat)
        {
            
            MenuItem cargar = new MenuItem() { Header = "Cargar" }, backup = new MenuItem() { Header = "Hacer BackUp" },quitarScript=new MenuItem() { Header="Quitar script"};
            int direccion;
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
                    direccion = MainWindow.Juego.ArchivoGbaPokemon.IndexOf(bytesScript);
                    if (direccion>0)
                    {
                        for (int i = direccion, j = 0; j < bytesScript.Length; i++, j++)
                            MainWindow.Juego.ArchivoGbaPokemon[i] = BYTEEMPTY;
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
                direccion = MainWindow.Juego.ArchivoGbaPokemon.IndexOf(bytesScript);
                if (direccion < 0)
                {
                    PonDireccionCorrecta();
                    PonBytes();
                    
                }
                else
                {
                    txtOffset.Text = (Hex)direccion;
                    MessageBox.Show("El scritp ja esta en la ROM!");
                }
            }

        }

        private void PonDireccionCorrecta()
        {
           if(!ValidaLugarDondeSePondra())
                txtOffset.Text = (Hex)MainWindow.Juego.ArchivoGbaPokemon.IndexOf(INICIO,bytesEmpty);
        }

        private void PonBytes()
        {
          for (int i = (Hex)txtOffset.Text, f = i + bytesScript.Length, pos = 0; i < f; i++, pos++)
              MainWindow.Juego.ArchivoGbaPokemon[i] = bytesScript[pos];
          MainWindow.Juego.Save();          
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
                try
                {
                    for (int i = offSet, j = 0; valida && j < bytesScript.Length; i++, j++)
                        valida = MainWindow.Juego.ArchivoGbaPokemon[i] == BYTEEMPTY;
                }
                catch { valida = false; }//si se sale de la matriz no es valida!
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
            if (MainWindow.IsEsmeralda.HasValue)
            {
                if (MainWindow.IsEsmeralda.Value)
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
                txtByteScript.Text = AplicarEnLaRom.BytesScript(MainWindow.IsEsmeralda.Value, pokemon, vida, nivel, stat);
                txtOffset.Text = "";

                bytesScript = GetBytes(txtByteScript.Text);
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

        public static string BytesScript(bool isEsmeralda, int pokemon, int vida, byte nivel, byte stat)
        {
            string script, variableQueToca, stringQueToca;
            stringQueToca = isEsmeralda ? MainWindow.VariableEspecialE.Split('x')[1] : MainWindow.VariableEspecialR.Split('x')[1];
            stringQueToca = stringQueToca.PadLeft(4, '0');
            script = "25 " + stringQueToca.Substring(2, 2) + " " + stringQueToca.Substring(0, 2);
            stringQueToca = isEsmeralda ? MainWindow.VariablePokemonE.Split('x')[1] : MainWindow.VariablePokemonR.Split('x')[1];
            variableQueToca = ((Hex)pokemon).Number.PadLeft(4, '0');
            script += "\r\n16 " + stringQueToca.Substring(2, 2) + " " + stringQueToca.Substring(0, 2) + " " + variableQueToca.Substring(2, 2) + " " + variableQueToca.Substring(0, 2);
            stringQueToca = isEsmeralda ? MainWindow.VariableVitalidadE.Split('x')[1] : MainWindow.VariableVitalidadR.Split('x')[1];
            variableQueToca = ((Hex)vida).Number.PadLeft(4, '0');
            script += "\r\n16 " + stringQueToca.Substring(2, 2) + " " + stringQueToca.Substring(0, 2) + " " + variableQueToca.Substring(2, 2) + " " + variableQueToca.Substring(0, 2);
            stringQueToca = isEsmeralda ? MainWindow.VariableNivelYEstadoE.Split('x')[1] : MainWindow.VariableNivelYEstadoR.Split('x')[1];
            script += "\r\n16 " + stringQueToca.Substring(2, 2) + " " + stringQueToca.Substring(0, 2) + " " + ((Hex)nivel).Number.PadLeft(2, '0') + " " + ((Hex)stat).Number.PadLeft(2, '0');
            script += "\r\n02 "+(isEsmeralda?"00":"FF");
            return script;
        }
    }
}

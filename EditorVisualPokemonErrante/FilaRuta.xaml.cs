using Gabriel.Cat;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gabriel.Cat.Extension;
namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para FilaRuta.xaml
    /// </summary>
    public partial class FilaRuta : UserControl
    {
        const int MAXCOLUMNAS = 7;
        readonly Hex MAXCAMPO = 255;
        TextBox[] campos;
        public event EventHandler Click;
        public FilaRuta()
        {
            MainWindow.JuegoUpdated += CambioJuego;
            InitializeComponent();
            campos = new TextBox[MAXCOLUMNAS];
            campos[0] = txtRuta;
            campos[1] = txtRebote1;
            campos[2] = txtRebote2;
            campos[3] = txtRebote3;
            campos[4] = txtRebote4;
            campos[5] = txtRebote5;
            campos[6] = txtRebote6;
            for (int i = 0; i < campos.Length; i++)
            {
                campos[i].MouseLeftButtonUp += (s, e) => { if (Click != null) Click(this, e); };
                campos[i].Text = "FF";
            }
            CambioJuego();
        }

        private void CambioJuego(object sender=null, EventArgs e=null)
        {
         
                if (MainWindow.Juego != null)
                {
                    IsEsmeraldaRow = MainWindow.Juego is Gabriel.Cat.GBA.RomEsmeralda;
                }
                for (int i = 0; i < campos.Length; i++)
                    campos[i].IsReadOnly = MainWindow.Juego == null;
            
        }

        public FilaRuta(byte[] columnas):this()
        {
            if (columnas.Length > campos.Length)
                throw new ArgumentOutOfRangeException("el numero de columnas es mas grande que la cantidad de campos!");
            for (int i = 0; i < columnas.Length; i++)
                campos[i].Text = (Hex)columnas[i];

        }

        public Hex[] Fila
        {
            get
            {
                Hex[] fila = new Hex[MAXCOLUMNAS-(IsEsmeraldaRow?1:0)];
                ValidaCampos();
                for (int i = 0; i < fila.Length; i++)
                    fila[i] = campos[i].Text;
                return fila;
            }
        }
        public byte[] Bytes
        {
            get
            {
                byte[] bytes = new byte[MAXCOLUMNAS - (IsEsmeraldaRow ? 1 : 0)];
                Hex[] fila = Fila;
                for (int i = 0; i < bytes.Length;i++)
                    bytes[i] =(byte)fila[i];
                return bytes;
            }
        }

        public bool IsEsmeraldaRow
        {
            get
            {
                return !txtRebote6.IsEnabled;
            }

            set
            {
                txtRebote6.IsEnabled = !value;
                if (IsEsmeraldaRow)
                    txtRebote6.Background = Brushes.LightGray;
                else
                    txtRebote6.Background = Brushes.LightGreen;
            }
        }

        private void ValidaCampos()
        {
            Hex numCampo;
            for (int i = 0; i < campos.Length; i++)
            {
                if (Hex.ValidaString(campos[i].Text))
                {
                    numCampo = campos[i].Text;
                    if (numCampo > MAXCAMPO)
                        campos[i].Text = MAXCAMPO;
                    else if (numCampo < 0)
                        campos[i].Text = "0";
                }
                else campos[i].Text = "FF";
            }
        }
        public byte[] ToByteArray()
        {
            byte[] campos=new byte[this.campos.Length];
            ValidaCampos();
            for (int i = 0; i < campos.Length; i++)
                campos[i] = Convert.ToByte((int)(Hex)this.campos[i].Text);
            return campos;
        }
        public static FilaRuta[] ToFilaRutaArray(byte[,] tablaRutas)
        {
            FilaRuta[] filas = new FilaRuta[tablaRutas.GetLength(DimensionMatriz.Fila)];
            for (int i = 0; i < filas.Length; i++)
                filas[i] = new FilaRuta(tablaRutas.Fila(i));
            return filas;
        }
        public static byte[,] ToByteMatriu(FilaRuta[] filas)
        {
            byte[,] matriz = new byte[MainWindow.Juego.ColumnasFilaPokemonErrante, filas.Length];
            byte[] filaByteArray;
         
            for (int i=0;i<filas.Length;i++)
            {
                filaByteArray = filas[i].ToByteArray();
                for (int j = 0, jFin = matriz.GetLength(DimensionMatriz.Columna); j < jFin; j++)
                    matriz[j, i] = filaByteArray[j];
            }
            return matriz;

        }
    }
}

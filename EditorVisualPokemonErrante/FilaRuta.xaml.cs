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
        public FilaRuta()
        {

            InitializeComponent();
            campos = new TextBox[MAXCOLUMNAS];
            campos[0] = txtRuta;
            campos[1] = txtRebote1;
            campos[2] = txtRebote2;
            campos[3] = txtRebote3;
            campos[4] = txtRebote4;
            campos[5] = txtRebote5;
            campos[6] = txtRebote6;
            for (int i = 0; i < campos.Length;i++)
                campos[i].MouseLeftButtonUp +=(s,e)=> { OnMouseLeftButtonUp(e); };
        }
        public FilaRuta(bool isEsmeraldaRow) : this()
        {
            IsEsmeraldaRow = isEsmeraldaRow;
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
                numCampo = campos[i].Text;
                if (numCampo > MAXCAMPO)
                    campos[i].Text = MAXCAMPO;
                else if (numCampo < 0)
                    campos[i].Text = "0";
            }
        }
    }
}

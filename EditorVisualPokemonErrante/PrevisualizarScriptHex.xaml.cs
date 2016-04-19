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
using System.Windows.Shapes;
using Gabriel.Cat.Extension;
namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para PrevisualizarScriptHex.xaml
    /// </summary>
    public partial  class PrevisualizarScriptHex : Window
    {
        public PrevisualizarScriptHex( int pokemon, int vida, byte nivel, byte stat)
        {
            PrevisualizarScriptXSE prvXSE = new PrevisualizarScriptXSE(pokemon, vida, nivel, stat);
            InitializeComponent();
            txtScriptR.Text = prvXSE.txtScriptR.Text.Remove(0,PrevisualizarScriptXSE.CABEZERASCRIPT.Length).Replace("special","0x25").Replace("setvar", "0x16").Replace("end", "0xFF");
            txtScriptE.Text = prvXSE.txtScriptE.Text.Remove(0, PrevisualizarScriptXSE.CABEZERASCRIPT.Length).Replace("special", "0x25").Replace("setvar", "0x16").Replace("end", "0xFF");
            imgVersionE.Source = prvXSE.imgVersionE.Source;
            imgVersionR.Source = prvXSE.imgVersionR.Source;
        }



    }
}

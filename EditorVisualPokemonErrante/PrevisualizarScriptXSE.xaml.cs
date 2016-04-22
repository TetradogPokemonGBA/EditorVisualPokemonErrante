using Gabriel.Cat;
using Gabriel.Cat.Extension;
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

namespace EditorVisualPokemonErrante
{
    /// <summary>
    /// Lógica de interacción para ExportarXSE.xaml
    /// </summary>
    public partial class PrevisualizarScriptXSE : Window
    {
        static Gabriel.Cat.GBA.RomEsmeralda romEsmeralda = new Gabriel.Cat.GBA.RomEsmeralda();
        static Gabriel.Cat.GBA.RomFRLG romRojoFuego = new Gabriel.Cat.GBA.RomFRLG();
        
        public PrevisualizarScriptXSE()
        {
            InitializeComponent();
        }

        public PrevisualizarScriptXSE(int pokemon,int vida,byte nivel,byte stat):this()
        {

            imgVersionR.SetImage(Resource1.FireRed);
            txtScriptR.Text = romRojoFuego.ScriptPokemonErrante(pokemon,vida,nivel,stat);
            imgVersionE.SetImage(Resource1.Emerald);
            txtScriptE.Text = romEsmeralda.ScriptPokemonErrante(pokemon, vida, nivel, stat);
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Basice.UI
{
    public partial class ExamplesBrowser : Form
    {
        private List<string> _examples;
        public string Program { get; private set; }

        private class Example
        {
            public int Id { get; }
            public string Title { get; }
            public string Description { get; }
            public string Concepts { get; }

            public Example(int id, string title, string description, string concepts)
            {
                Id = id;
                Title = title;
                Description = description;
                Concepts = concepts;
            }
        }

        public ExamplesBrowser()
        {
            InitializeComponent();
        }

        private void ExamplesBrowser_Load(object sender, EventArgs e)
        {
            Program = "";
            _examples = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(z => z.StartsWith("Basice.UI.Examples.")).ToList();
            var examplesList = new List<Example>();
            
            for (var i = 0; i < _examples.Count; i++)
            {
                string example = _examples[i];
                string prog = "";

                using (var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(example)))
                {
                    prog = sr.ReadToEnd();
                    sr.Close();
                }

                prog = prog.Replace("\r", "");
                string[] lines = prog.Split('\n');
                string title = lines[0];
                string desc = lines[1];
                string concepts = lines[2];

                examplesList.Add(new Example(i, title, desc, concepts));
            }

            foreach (Example example in examplesList.OrderBy(z => z.Title))
            {
                DgExamples.Rows.Add(example.Id, example.Title, example.Description, example.Concepts);
            }
        }

        private void DgExamples_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = (int)DgExamples.Rows[e.RowIndex].Cells[0].Value;

            using (var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(_examples[rowIndex])))
            {
                Program = sr.ReadToEnd();
                Program = Program.RemoveFirstLines(3);
                sr.Close();
            }

            DialogResult = DialogResult.OK;
        }
    }
}

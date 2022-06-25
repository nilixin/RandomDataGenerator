using System.Linq;

namespace RandGen
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void bGenerate_Click(object sender, EventArgs e)
        {
            tbOutput.Text = null;
            string input = tbInput.Text.Replace("\r", "");
            string query = tbBefore.Text;
            string after = tbAfter.Text;

            try
            {
                int linesGenerated = 0;
                for (int i = 0; i < nudIterations.Value; i++)
                {
                    Fill(input, query, after);
                    linesGenerated = i;
                }

                if (linesGenerated > 1000)
                {
                    MessageBox.Show($"Generating done. Generated {linesGenerated}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bFilter_Click(object sender, EventArgs e)
        {
            try
            {
                Filter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bReshuffle_Click(object sender, EventArgs e)
        {
            string output = tbOutput.Text;
            List<string> initialLines = output.Split("\r\n").ToList();
            initialLines.RemoveAt(initialLines.Count - 1);

            List<string> reshuffledLines = new List<string>();

            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                reshuffledLines = initialLines.OrderBy(item => random.Next()).ToList();
            }

            string reshuffledOutput = string.Join("\r\n", reshuffledLines);

            if (MessageBox.Show($"Are you sure to reshuffled all of the output lines?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                tbOutput.Text = reshuffledOutput;
            }
        }

        private void bHelp_Click(object sender, EventArgs e)
        {
            Form form = new Form();
            form.Width = 500;
            form.Height = 400;
            form.Controls.Add(new Label()
            {
                Width = form.Width,
                Height = form.Height,
                Text = "Note:\n" +
                "new line - creates new attributes('ATTRIBUTE', 'ATTRIBUTE')\n" +
                "| -separates value combinations\n" +
                "; -separates values\n" +
                "/## - ONLY at the beginning of a line, puts in double quotes (\"TXT\")\n" +
                "/# - ONLY at the beginning of a line, puts in quotes ('TXT')\n" +
                "/* - ONLY at the beginning of a line, only separates with a comma, useful for numbers (00, 00, 00)\n" +
                "/ _ - ONCE per option, puts a space after option('OPT1 OPT2')\n" +
                "/ __ - ONCE per option, puts a space before option('OPT2 OPT1')\n\n" +
                "Filtering by 0 means filtering by the entire line.\n" +
                "Filtering by attribute is done ONLY if an attribute is in singular quotes ('ATTRIBUTE')"
            });
            form.Show();
        }

        private void Fill(string input, string query, string after)
        {
            List<string> atts = input.Split('\n').ToList();

            for (int j = 0; j < atts.Count; j++) // for each attribute in attributes
            {
                if (atts[j].Contains("/##")) // if "##" at the beginning of a line
                {
                    atts[j] = atts[j].Replace("/##", "");
                    query = NewAttribute(query, "\"", atts[j]);
                    if (j != atts.Count - 1)
                        query += ",";
                }
                else if (atts[j].Contains("/#")) // if "#" at the beginning of a line
                {
                    atts[j] = atts[j].Replace("/#", "");
                    query = NewAttribute(query, "'", atts[j]);
                    if (j != atts.Count - 1)
                        query += ",";
                }
                else if (atts[j].Contains("/*")) // if "*" at the beginning of a line
                {
                    atts[j] = atts[j].Replace("/*", "");
                    query = NewAttribute(query, "", atts[j]);
                    if (j != atts.Count - 1)
                        query += ",";
                }
                else
                {
                    query = NewAttribute(query, "", atts[j]);
                }
            }

            query = query.Replace("  ", " ");
            query = query.Replace(" '", "'");
            query = query.Replace("' ", "'");
            //query = query.Replace(" ,", ",");
            //query = query.Replace("/#", "");
            //query = query.Replace("/##", "");
            //query = query.Replace("/_", "");
            //query = query.Replace("/_", "");

            tbOutput.Text += query + after + "\r\n";
        }

        private string NewAttribute(string query, string wrapper, string line)
        {
            query += wrapper;

            List<string> opts = line.Split('|').ToList();
            int count = opts.Count;
            for (int k = 0; k < count; k++) // for each option in options
            {
                bool containsSpaceBefore = opts[k].Contains("/__");
                bool containsSpaceAfter = false;
                if (!containsSpaceBefore)
                    containsSpaceAfter = opts[k].Contains("/_");
                opts[k] = opts[k].Replace("/__", "");
                opts[k] = opts[k].Replace("/_", "");

                if (containsSpaceBefore)
                    query += " ";

                query = NewOption(query, opts[k], count);

                if (containsSpaceAfter)
                    query += " ";
            }

            query += wrapper;
            
            return query;
        }

        private string NewOption(string query, string option, int count)
        {
            Random random = new Random();

            List<string> vals = option.Split(';').ToList();
            int index = random.Next(0, vals.Count);
            string val = vals[index];
            query += val;
            
            return query;
        }

        private void Filter()
        {
            int filterBy = (int)nudAttributes.Value;
            List<string> filteredByLines = new List<string>();

            string output = tbOutput.Text;

            List<string> lines = output.Split("\r\n").ToList();
            int initialLinesCount = lines.Count;

            if (filterBy == 0)      // filtering the entire line
            {
                for (int t = 0; t < 2; t++)                                         // 0 two times
                    for (int i = 0; i < lines.Count - 1; i++)                       // 1 for each line
                        for (int j = 0; j < lines.Count - 1; j++)                   // 2 for each other line
                            if (j != i)                                             // 3 if it is not the same one
                                if (lines[i] == lines[j])                           // 4 if these lines are similar
                                    lines.RemoveAt(j);                              // 5 then remove the latter

                int finalLinesCount = lines.Count;
                int countDifference = initialLinesCount - finalLinesCount;

                string filteredOutput = string.Join("\r\n", lines);

                if (MessageBox.Show($"Are you sure to remove {countDifference} lines?\n\n" +
                $"Lines initially: {initialLinesCount}\n" +
                $"Lines finally: {finalLinesCount}", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    tbOutput.Text = filteredOutput;
                }
            }
            else                    // filtering by one attribute
            {
                for (int t = 0; t < 2; t++)                                         // 0 two times
                    for (int i = 0; i < lines.Count - 1; i++)                       // 1 for each line
                    {
                        List<string> attsi = lines[i].Split('\'').ToList();         // 2 determine the attributes of one line
                        for (int ii = 0; ii < attsi.Count; ii++)
                            if (ii == 0 || ii % 2 != 0)
                                attsi.RemoveAt(ii);

                        for (int j = 0; j < lines.Count - 1; j++)                   // 3 for each other line
                        {
                            List<string> attsj = lines[j].Split('\'').ToList();     // 4 determine the attributes of the other line
                            for (int jj = 0; jj < attsj.Count; jj++)
                                if (jj == 0 || jj % 2 != 0)
                                    attsj.RemoveAt(jj);

                            if (j != i)                                             // 5 if it is not the same line
                                if (attsi[filterBy - 1] == attsj[filterBy - 1])     // 6 if the attributes of the corresponding index are similar
                                {
                                    filteredByLines.Add(lines[i]);
                                    filteredByLines.Add(lines[j]);
                                    lines.RemoveAt(j);                              // 7 then remove the latter
                                }
                        }
                    }

                int finalLinesCount = lines.Count;
                int countDifference = initialLinesCount - finalLinesCount;

                string filteredOutput = string.Join("\r\n", lines);

                string filteredByLinesString = string.Empty;
                try
                {
                    filteredByLinesString = string.Join("\n", filteredByLines.ToArray(), startIndex: 0, count: 4);
                }
                catch (Exception) { }

                if (MessageBox.Show($"Are you sure to remove {countDifference} lines?\n\n" +
                $"Lines initially: {initialLinesCount}\n" +
                $"Lines finally: {finalLinesCount}\n\n" +
                $"{filteredByLinesString}\n...", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    tbOutput.Text = filteredOutput;
                }
            }
        }
    }
}
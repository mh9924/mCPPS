using System.IO;
using System.Text;
using System.Windows.Forms;

public class TextBoxConsole : TextWriter
{
    TextBox output = null; 
    
    public TextBoxConsole(TextBox _output)
    {
        output = _output;
        output.ScrollBars = ScrollBars.Both;
        output.WordWrap = true;
    }

    public override void Write(char value)
    {
        base.Write(value);
        output.AppendText(value.ToString());
    }

    public override Encoding Encoding
    {
        get { return System.Text.Encoding.UTF8; }
    }
}

using System.Windows;

namespace SyntaxHighlighting
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            tb.Json = @"{
    'title': 'Person',
    'type': 'object',
    'properties': {
        'firstName': {
            'type': 'string'
        },
        'lastName': {
            'type': 'string'
        },
        'age': {
            'description': 'Age in years',
            'type': 'integer',
            'minimum': 0
        }
    },
    'required': ['firstName', 'lastName']
}".Replace('\'', '"');
        }
    }
}
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Progetto2
{
    [DesignTimeVisible(false)]

    // La classe MainPage è una classe parziale che deriva da ContentPage
    public partial class MainPage : ContentPage
    {
        public MainPage() //Il costruttore viene richiamato ogni volta che si carica la pagina
        {
            // Questo metodo è definito all'interno di un altro file contenente la seconda parte della definizione
            // della classe parziale MainPage. Permette di inizializzare le componenti della pagina in base alla piattaforma.
            InitializeComponent();
        }

        //Evento che viene richiamato al click del bottone "Cerca Film"
        private void Cerca_Film_Cliccato(object sender, EventArgs e)
        {
            // Quando il bottone viene cliccato aggiungo in modo asincrono la pagina CercaFilm all'inizio dello stack di navigazione
            Navigation.PushAsync(new CercaFilm());
        }

    }
}

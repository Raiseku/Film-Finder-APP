using System;
using System.IO;
using System.Net;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Progetto2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class dettaglioSingoloFilm : ContentPage
    {

        // Variabile globale con il titolo del film scelto. 
        // Tramite questa variabile sarà possibile ottenere informazioni all'interno del sito
        string filmScelto; 

        public dettaglioSingoloFilm(string filmScelto_passato)
        {
            this.filmScelto = filmScelto_passato;
            InitializeComponent();

            //Richiamo il metodo caricaDettaglio
            caricaDettaglio();
        }

        //Definisco il metodo carica dettaglio
        public void caricaDettaglio(){

            //Creo un oggetto della classe webClient che mi permetterà di richiamare metodi per l'invio o la ricezione di dati da internet.
            //var rappresenta un tipo implicito, sarà il compilatore a determinare il tipo a tempo di compilazione
            //Essendo all'interno di un blocco di 'using' verrà richiamato il metodo Dispose() in uscita, questo permetterà alla
            //Garbage Collector di deallocare l'oggetto.
            using (var wc = new WebClient()){
                try{
                    //Definisco l'apikey e le variabili che conterranno le informazioni sul film.
                    string apikey = "3ee7f8c9";
                    string titolo = "";
                    string dataRilascio = "";
                    string durata = "";
                    string trama = "";
                    string url_locandina = "";

                    //Definisco l'url del sito per il download dei dettagli del film scelto concatenando il titolo del film,
                    //l'apikey per l'utilizzo del servizio e scelgo come formato l'xml.
                    string url = "http://www.omdbapi.com/?t=" + filmScelto + "&r=xml&apikey=" + apikey;
                    
                    //Definisco il nome del file al quale interno verrà scaricato il contenuto dal sito
                    string nomeFile = "dettaglioFilm.xml";

                    string locazione_xml = "";

                    // La locazione del File è diversa in base alla piattaforma sulla quale sta venendo eseguita l'applicazione.
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        //Nel caso in cui l'app fosse eseguita su Android allora la cartella scelta è Personal
                        locazione_xml = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    }
                    else if (Device.RuntimePlatform == Device.UWP)
                    {
                        //Se invece l'applicazione fosse eseguita su UWP allora la cartella scelta è LocalApplicationData.
                        locazione_xml = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    }
                    
                    //Ottengo il path completo combinando la locazione ed il nome del file
                    string path_completo = Path.Combine(locazione_xml, nomeFile);

                    //Effettuo il download del file
                    wc.DownloadFile(url, path_completo);

                    //Salvo il testo contenuto all'interno del file su una variabile di tipo string
                    string stringaListaFilm = File.ReadAllText(path_completo);

                    //Creo un oggetto della classe XmlDocument per la gestione di documenti XML
                    XmlDocument xDoc = new XmlDocument();

                    //Carico il contenuto della stringa all'interno dell'oggetto xDoc
                    xDoc.LoadXml(stringaListaFilm);

                    //Scorro tutti i nodi all'interno del documento xml
                    foreach (XmlNode node in xDoc.DocumentElement){
                        try
                        {
                            //Ottengo i valori necessari compresa la locandina
                            titolo = node.Attributes["title"].Value;
                            dataRilascio = node.Attributes["released"].Value;
                            durata = node.Attributes["runtime"].Value;
                            trama = node.Attributes["plot"].Value;
                            url_locandina = node.Attributes["poster"].Value;
                        }
                        catch (NullReferenceException ex)
                        {
                            Console.WriteLine("Eccezione NullReferenceException" + ex.Message);
                            DisplayAlert("Attenzione", "Il dettaglio del film selezionato non è disponibile", "Ok");
                        }
                    }

                    //Inserisco i valori precedenti all'interno delle rispettive Label
                    LabeltitoloFilm.Text = titolo;
                    LabeldataRilascio.Text = "Data di Rilascio: " + dataRilascio;
                    Labeldurata.Text = "Durata: " + durata;
                    Labeltrama.Text = "Trama: " + trama;

                    try
                    {
                        // Controllo che l'uri della locandina sia valido, nel caso in cui non lo fosse
                        // viene generata un'eccezione con il display alert di un errore
                        ImageLocandina.Source = ImageSource.FromUri(new Uri(url_locandina));

                        // Se l'url della locandina è valido continuo l'esecuzione e definisco il suo nome
                        string nomeLocandina = "locandina.png";
                        string locazioneLocandina = "";

                        // La locazione del File è diversa in base alla piattaforma sulla quale sta venendo eseguita l'applicazione.
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            //Nel caso in cui l'app fosse eseguita su Android allora la cartella scelta è Personal
                            locazioneLocandina = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        }
                        else if (Device.RuntimePlatform == Device.UWP)
                        {
                            //Se invece l'applicazione fosse eseguita su UWP allora la cartella scelta è LocalApplicationData.
                            locazioneLocandina = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        }
                        
                        //Definisco il path completo della locandina combinando posizione e nome della locandina
                        string path_completo_locandina = Path.Combine(locazioneLocandina, nomeLocandina);

                        //Effettuo il download della locandina
                        wc.DownloadFile(url_locandina, path_completo_locandina);

                        //Imposto la source della locandina
                        ImageLocandina.Source = path_completo_locandina;
                    }
                    catch (UriFormatException uriEx){
                        //Se la locandina non è presente allora non la mostro ed effettuo il display di un errore.
                        Console.WriteLine("Eccezione eriEx" + uriEx.Message);
                        App.Current.MainPage.DisplayAlert("Attenzione", "Locandina non disponibile.", "Ok");
                    }
                }
                catch (ArgumentException argNull){
                    Console.WriteLine("Eccezione argNull" + argNull.Message);
                }
                catch (WebException webEx){
                    Console.WriteLine("Eccezione webEx" + webEx.Message);
                }
                catch (NotSupportedException notEx){
                    Console.WriteLine("Eccezione notSuppEx" + notEx.Message);
                }
            }
        }
    }
}
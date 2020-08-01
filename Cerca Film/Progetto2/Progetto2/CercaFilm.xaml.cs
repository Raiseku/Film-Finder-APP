using Progetto2.Modelli; //Importo la cartella per poter utilizzare le classi al suo interno
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Progetto2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CercaFilm : ContentPage
    {
        //Variabile globale che avrà al suo interno il percorso del file xml
        //contenente tutte le informazioni sui film scaricate da internet
        string path_completo;

        public CercaFilm() //Il costruttore viene richiamato ogni volta che si carica la pagina
        {
            InitializeComponent();
        }

        //Metodo che verrà richiamato al click del bottone "Cerca_Cliccato"
        private void bottoneCerca_Cliccato(object sender, EventArgs e)
        {
            //Ottengo la parola inserita dall'utente e controllo se è vuota
            string parolaCercata = nomeCercato.Text;

            //Se la parola è vuota non effetto nessuna richiesta al sito e ritorno un errore all'utente.
            if (string.IsNullOrEmpty(parolaCercata)) {
                DisplayAlert("Attenzione", "Inserisci una parola.", "Ok");
            }

            // Altrimenti creo un oggetto della classe WebClient che mi permetterà di richiamare metodi per l'invio o la ricezione
            // di dati da internet.
            // var rappresenta un tipo implicito, sarà il compilatore a determinare il tipo a tempo di compilazione.
            // Essendo 'wc' all'interno di un blocco di 'using' verrà richiamato automaticamente il metodo Dispose in uscita,
            // questo permetterà alla Garbage Collector di deallocare l'oggetto.
            else
            {
                using (var wc = new WebClient()){
                    try{ 
                        //Definisco la key per utilizzare le API del sito. La key è stata ottenuta dal sito stesso dopo
                        //la registrazione
                        string apikey = "3ee7f8c9";

                        /*Creo una variabile di tipo string che conterrà l'url del sito.
                         Per effettuare correttamente la ricerca va inserita la parola cercata dall'utente, il formato
                         con il quale visualizzare i dati e l'apiKey */
                        string url = "http://www.omdbapi.com/?s=" + parolaCercata + "&r=xml&apikey=" + apikey;

                        Uri uri = new Uri(url); //Creo l'uri 
                        string nomeFile = "ListaFilm.xml"; //Definisco il nome del file il cui contenuto sarà scaricato da internet

                        string locazioneFile = "";

                        // La locazione del File è diversa in base alla piattaforma sulla quale sta venendo eseguita l'applicazione.
                        if(Device.RuntimePlatform == Device.Android)
                        {
                            //Nel caso in cui l'app fosse eseguita su Android allora la cartella scelta è Personal
                            locazioneFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        }
                        else if (Device.RuntimePlatform == Device.UWP)
                        {
                            //Se invece l'applicazione fosse eseguita su UWP allora la cartella scelta è LocalApplicationData.
                            locazioneFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        }

                        //Definisco il path completo combinando la locazione ed il nome del file
                        path_completo = Path.Combine(locazioneFile, nomeFile);

                        //Durante il progresso del download aggiorno la progressBar tramite il metodo gestioneProgresso
                        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(gestioneProgresso);

                        //Quando il download è terminato, viene richiamato il metodo gestioneCompletamento
                        wc.DownloadFileCompleted += new AsyncCompletedEventHandler(gestioneCompletamento);

                        //Effettuo il download del file in maniera Asincrona cosi da lasciare libero il main thread
                        wc.DownloadFileAsync(uri, path_completo);
                    }


                    //Gestisco tutte le eccezioni che potrebbero essere sollevate dalle chiamate precedenti
                    catch (ArgumentException argNull){
                        Console.WriteLine("Eccezione argNull" + argNull.Message);
                        DisplayAlert("Errore", "ArgumentException:" + argNull.Message, "Ok");
                    }
                    catch (WebException webEx){
                        Console.WriteLine("Eccezione webEx" + webEx.Message);
                        DisplayAlert("Errore", "Eccezione webEx" + webEx.Message, "Ok");
                    }
                    catch (NotSupportedException notEx){
                        Console.WriteLine("Eccezione notSuppEx" + notEx.Message);
                        DisplayAlert("Errore", "Eccezione notSuppEx" + notEx.Message, "Ok");
                    }
                }
            }
        }

        //Metodo che verrà richiamato durante il download del file da internet
        private void gestioneProgresso(object sender, DownloadProgressChangedEventArgs e)
        {
            //Aggiorno la progress bar in base ai bytes ricevuti
            progBar.Progress = e.BytesReceived;
        }


        //Metodo che verrà richiamato dopo che il file xml è stato scaricato da internet
        private void gestioneCompletamento(object sender, AsyncCompletedEventArgs e){
            
            //Azzero il progresso della barra
            progBar.Progress = 0;
            
            //Leggo il file passando il suo path_completo e lo salvo in una stringa
            string stringaListaFilm = File.ReadAllText(path_completo);

            //Creo un oggetto della classe XmlDocument che mi permetterà di gestire in memoria un documento XML
            XmlDocument xDoc = new XmlDocument();

            //Carico la stringa all'interno dell'oggetto xDoc 
            xDoc.LoadXml(stringaListaFilm);

            //Creo una lista di oggetti di tipo Film
            List<Film> ArrayListFilm = new List<Film>();

            //Scorro tutti i nodi all'interno del documento XML:
            foreach (XmlNode node in xDoc.DocumentElement){
                try{
                    //Ottengo il titolo e l'anno in base al tag xml
                    string titolo = node.Attributes["title"].Value;
                    string anno = node.Attributes["year"].Value;

                    //Creo un nuovo oggetto della classe Film con i dati ottenuti precedentemente
                    Film film = new Film(titolo, anno);

                    //Aggiungo il film alla lista 
                    ArrayListFilm.Add(film);
                }

                //Se la lista che ritorna il sito è vuota significa che non è stato trovato nessun film con la parola
                //inserita dall'utente, genero un DisplayAlert.
                catch (NullReferenceException ex){
                    Console.WriteLine("Eccezione NullReferenceException" + ex.Message);
                    DisplayAlert("Attenzione", "Nessun film trovato con il nome inserito. ", "Ok");
                }
            }

            // A questo punto tutti i film trovati sono stati caricati all'interno della lista 'ArrayListFilm'.
            // Si imposta come sorgente della listview l'ArrayListFim, così facendo si visualizzano tutti i film trovati.
            listaFilm.ItemsSource = ArrayListFilm;
        }

        //Metodo che viene richiamato al click dell'utente su un'item della listView
        private void listaFilm_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Ottengo l'oggetto 'film' scelto dall'utente
            var filmScelto = e.Item as Film;

            // Aggiungo in modo asincrono la pagina CercaFilm all'inizio
            // dello stack di navigazione passando come parametro il titolo del film che l'utente ha scelto 
            Navigation.PushAsync(new dettaglioSingoloFilm(filmScelto.titoloFilm));

            //Tolgo l'highlight sull'item cliccato
            if (sender is ListView lv) lv.SelectedItem = null;
        }

    }
}
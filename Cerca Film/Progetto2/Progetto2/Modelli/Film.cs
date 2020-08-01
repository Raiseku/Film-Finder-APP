
namespace Progetto2.Modelli{

    /*Classe che descrive le caratteristiche di un film quando verrà mostrato all'interno della listview.
      In questo caso vengono mostrati il titolo e la data d'uscita.*/
    public class Film{

        /*Definisco le proprietà di get e set per ogni attributo in maniera automatica*/
        public string titoloFilm { get; set; }
        public string dataUscita { get; set; }

        /*Costruttore della classe*/
        public Film(string titoloFilm, string dataUscita){
            this.titoloFilm = titoloFilm;
            this.dataUscita = dataUscita;
        }

    }
}

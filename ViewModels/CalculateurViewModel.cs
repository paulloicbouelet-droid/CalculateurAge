namespace CalculateurAge.ViewModels;

// Contient l'ÉTAT de l'écran et les ACTIONS possibles.
public class CalculateurViewModel : BaseViewModel
{
    // Champs privés : la vraie donnée.
    private string _nom = "";
    private DateTime _dateNaissance = DateTime.Today.AddYears(-20);
    private string _resultat = "";
    private bool _resultatVisible;
    private string _message = "";
    private string _joursAnniversaire = "";

    // Propriétés publiques : ce que le XAML voit.
    public string Nom
    {
        get => _nom;
        set { if (SetField(ref _nom, value))
                  CalculerCommand.Rafraichir(); }
    }

    public DateTime DateNaissance
    {
        get => _dateNaissance;
        // Le bouton dépend aussi de la date (refus d'une date future).
        set { if (SetField(ref _dateNaissance, value))
                  CalculerCommand.Rafraichir(); }
    }

    public string Resultat
    {
        get => _resultat;
        set => SetField(ref _resultat, value);
    }

    public bool ResultatVisible
    {
        get => _resultatVisible;
        set => SetField(ref _resultatVisible, value);
    }

    // Fonctionnalité 1 : "Majeur" ou "Mineur".
    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    // Fonctionnalité 3 : jours restants avant le prochain anniversaire.
    public string JoursAnniversaire
    {
        get => _joursAnniversaire;
        set => SetField(ref _joursAnniversaire, value);
    }

    // Liées aux Button.Command dans le XAML.
    public RelayCommand CalculerCommand { get; }
    public RelayCommand EffacerCommand { get; }   // Fonctionnalité 2

    public CalculateurViewModel()
    {
        // Calculer : nom non vide ET date de naissance non future.
        CalculerCommand = new RelayCommand(
            Calculer,
            () => !string.IsNullOrWhiteSpace(Nom)
                  && DateNaissance.Date <= DateTime.Today);

        EffacerCommand = new RelayCommand(Effacer);
    }

    // La logique métier : aucun contrôle d'interface ici.
    private void Calculer()
    {
        DateTime aujourdhui = DateTime.Today;

        int age = aujourdhui.Year - DateNaissance.Year;
        if (DateNaissance.Date > aujourdhui.AddYears(-age)) age--;

        Resultat = $"{Nom}, vous avez {age} ans";
        Message = age >= 18 ? "Majeur" : "Mineur";
        JoursAnniversaire = CalculerJoursAvantAnniversaire(aujourdhui);
        ResultatVisible = true;
    }

    // Remet tous les champs à zéro.
    private void Effacer()
    {
        Nom = "";
        DateNaissance = DateTime.Today.AddYears(-20);
        Resultat = "";
        Message = "";
        JoursAnniversaire = "";
        ResultatVisible = false;
    }

    private string CalculerJoursAvantAnniversaire(DateTime aujourdhui)
    {
        DateTime prochain = AnniversairePourAnnee(aujourdhui.Year);
        if (prochain < aujourdhui)
            prochain = AnniversairePourAnnee(aujourdhui.Year + 1);

        int jours = (prochain - aujourdhui).Days;
        return jours == 0
            ? "Bon anniversaire, c'est aujourd'hui !"
            : $"Prochain anniversaire dans {jours} jour(s)";
    }

    // Gère le 29 février les années non bissextiles (-> 28 février).
    private DateTime AnniversairePourAnnee(int annee)
    {
        int jour = DateNaissance.Day;
        if (DateNaissance.Month == 2 && jour == 29
            && !DateTime.IsLeapYear(annee))
            jour = 28;
        return new DateTime(annee, DateNaissance.Month, jour);
    }
}

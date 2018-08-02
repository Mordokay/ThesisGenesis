using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {

    public GameObject SwitchLanguageButton;
    bool isPTLanguage;

    public GameObject prophetBaloon;

    public int tutorialStage;
    public GameObject[] tutorials;

    bool usedW;
    bool usedA;
    bool usedS;
    bool usedD;

    public Text TextW;
    public Text TextA;
    public Text TextS;
    public Text TextD;

    public Text prisonWarning;

    void Start () {
        tutorialStage = 0;
        usedW = false;
        usedA = false;
        usedS = false;
        usedD = false;

        isPTLanguage = false;
    }
	
    public void NextTutorial()
    {
        tutorialStage += 1;
        if (tutorialStage < tutorials.Length)
        {
            RefreshTutorialTexts();
        }
    }

    public void SwitchLanguage()
    {
        if (isPTLanguage)
        {
            isPTLanguage = !isPTLanguage;
            SwitchLanguageButton.GetComponentInChildren<Text>().text = "EN";
            prisonWarning.text = "Next Time" + System.Environment.NewLine + "be more" + System.Environment.NewLine + "carefull!";

            //puts in english
            tutorials[0].GetComponentInChildren<Text>().text = "Oh hi! ... didn't see you there! Where did you come from?" + System.Environment.NewLine;
            tutorials[0].transform.GetChild(1).GetComponentInChildren<Text>().text = "Dont Know";
            
            tutorials[1].GetComponentInChildren<Text>().text = " ... well  it doesn't matter. People call me the prophet and right now I am in urgent need of help!!!";
            tutorials[1].transform.GetChild(1).GetComponentInChildren<Text>().text = "I can Help!";

            tutorials[2].GetComponentInChildren<Text>().text = "What you can see behind me is the temple of souls and unless you get the golden relics to the altar this world is going to be destroyed.";
            tutorials[2].transform.GetChild(1).GetComponentInChildren<Text>().text = "Oh No!";

            tutorials[3].GetComponentInChildren<Text>().text = " ... yeah its very sad. I used to be part of this world until the guardians got corrupted by the darkness and started mind controlling the villagers ...";
            tutorials[3].transform.GetChild(1).GetComponentInChildren<Text>().text = "So sad ...";

            tutorials[4].GetComponentInChildren<Text>().text = "anyways ... since all the guardians are after me, I can't get all the relics myself, so I need you to bring me around 12 relics without being caught";
            tutorials[4].transform.GetChild(1).GetComponentInChildren<Text>().text = "Sure!";

            tutorials[5].GetComponentInChildren<Text>().text = "Before you start your journey I have to teach you a few very important things.";
            tutorials[5].transform.GetChild(1).GetComponentInChildren<Text>().text = "Teach Me!";

            tutorials[6].transform.GetChild(0).GetComponentInChildren<Text>().text = "To move around the map you Have to use they keys" + System.Environment.NewLine + "Try using them now!";

            tutorials[7].GetComponentInChildren<Text>().text = "Awesome! Now try to use your axe by pressing the left mouse button ...";

            tutorials[8].GetComponentInChildren<Text>().text = "You are doing great! Now that you know the basics lets try and grab that wood relic close to you ...";

            tutorials[9].GetComponentInChildren<Text>().text = "You can see that a villager was watching you gathering that relic!" + System.Environment.NewLine + "Vilagers cannot attack you but they can tell the guardians what you are doing, so be carefull!";
            tutorials[9].transform.GetChild(1).GetComponentInChildren<Text>().text = "Okey";

            tutorials[10].GetComponentInChildren<Text>().text = "anyways ... As you can see on the top right of the screen, the gold relic you just grabbed is now inside your stash.";
            tutorials[10].transform.GetChild(1).GetComponentInChildren<Text>().text = "I see";

            tutorials[11].GetComponentInChildren<Text>().text = "Now drop the relic at the altar on the center of the temple ...";

            tutorials[12].GetComponentInChildren<Text>().text = "You did it! We are 1 relic closer to the ultimate sacrifice!" + System.Environment.NewLine + "PS: You can also check your progress on the white bar on the right.";
            tutorials[12].transform.GetChild(1).GetComponentInChildren<Text>().text = "Nice!";

            tutorials[13].GetComponentInChildren<Text>().text = "If you are being chased by a guardian you can always escape with a quick dash. ";
            tutorials[13].transform.GetChild(1).GetComponentInChildren<Text>().text = "Cool!";

            tutorials[14].GetComponentInChildren<Text>().text = "To use the \"dash\" simply press space or the right mouse button while pointing with the mouse at the direction you want to go!  Try it ...";

            tutorials[15].GetComponentInChildren<Text>().text = "You are all set to go! Remember that your stash can't carry more than 4 relics. Drop the relics at the altar to free some space!";
            tutorials[15].transform.GetChild(1).GetComponentInChildren<Text>().text = "Got it!";

            tutorials[16].GetComponentInChildren<Text>().text = "Farewell traveler. I am counting on you! Dont let me down!";
            tutorials[16].transform.GetChild(1).GetComponentInChildren<Text>().text = "Farewell!";
            
        }
        else
        {
            isPTLanguage = !isPTLanguage;
            SwitchLanguageButton.GetComponentInChildren<Text>().text = "PT";
            prisonWarning.text = "Mais cuidado" + System.Environment.NewLine + "para a" + System.Environment.NewLine + "proxima!";
            //puts in portuguese

            tutorials[0].GetComponentInChildren<Text>().text = "Oh ola! Nao te vi ai!. De onde viste?";
            tutorials[0].transform.GetChild(1).GetComponentInChildren<Text>().text = "Nao Sei";

            tutorials[1].GetComponentInChildren<Text>().text = " ... agora nao importa. As pessoas chamam-me profeta e neste momento eu preciso da tua ajuda!!!";
            tutorials[1].transform.GetChild(1).GetComponentInChildren<Text>().text = "Claro!";

            tutorials[2].GetComponentInChildren<Text>().text = "O que ves atraz de mim e o Templo das Almas e se nao conseguires trazer todas as reliquias douradas para o altar este mundo vai ser destruido";
            tutorials[2].transform.GetChild(1).GetComponentInChildren<Text>().text = "Oh Nao :(";

            tutorials[3].GetComponentInChildren<Text>().text = " ... sim e muito triste. EU fazia parte deste mundo ate ao momento em que os guardioes ficaram corrompidos para escuridao e comecaram a controlar os aldeoes ...";
            tutorials[3].transform.GetChild(1).GetComponentInChildren<Text>().text = "Tao triste ...";

            tutorials[4].GetComponentInChildren<Text>().text = "... como todos os guardioes estao atraz de mim eu nao consigo apanhar as reliquias todas, portanto preciso que tu me recolhas 12 reliquias sem ser apanhado";
            tutorials[4].transform.GetChild(1).GetComponentInChildren<Text>().text = "Compreendo";

            tutorials[5].GetComponentInChildren<Text>().text = "Antes de comecares a tua viagem preciso de te ensinar algumas coisas";
            tutorials[5].transform.GetChild(1).GetComponentInChildren<Text>().text = "OK!";

            tutorials[6].transform.GetChild(0).GetComponentInChildren<Text>().text = "Para andar pelo mapa usa as teclas do teu teclado" + System.Environment.NewLine + "Tenta usa-las agora!";

            tutorials[7].GetComponentInChildren<Text>().text = "Perfeito! Agora tenta usar o machado pressionando o botao esquerdo do mapa ...";

            tutorials[8].GetComponentInChildren<Text>().text = "Boa! Agora que conheces os controlos basicos tenta apanhar a reliquia de madeira que esta perto de ti ...";

            tutorials[9].GetComponentInChildren<Text>().text = "Consegues ver que um aldeao viu-te a apanhar a reliquia!" + System.Environment.NewLine + "Os aldeoes nao te atacam mas podem ir contar ao guardiao mais proximo o que estavas a fazer, portanto tem cuidado!";
            tutorials[9].transform.GetChild(1).GetComponentInChildren<Text>().text = "OK";

            tutorials[10].GetComponentInChildren<Text>().text = "bem ... como podes ver no canto superior direito, a reliquia que apanhaste ja se encontra dentro do teu \"Stash\"";
            tutorials[10].transform.GetChild(1).GetComponentInChildren<Text>().text = "Eu vejo";

            tutorials[11].GetComponentInChildren<Text>().text = "Agora retorna a reliquia ao altar que se encontra no centro do templo ...";

            tutorials[12].GetComponentInChildren<Text>().text = "Conseguiste! Estamos 1 reliquia mais perto do sacrificio final!" + System.Environment.NewLine + "PS: Podes ver o progresso total na barra branca a direita do ecra.";
            tutorials[12].transform.GetChild(1).GetComponentInChildren<Text>().text = "Boa!";

            tutorials[13].GetComponentInChildren<Text>().text = "Se tiveres a ser perseguido por um guardiao podes tentar escapar usando o \" Dash\". ";
            tutorials[13].transform.GetChild(1).GetComponentInChildren<Text>().text = "Como?";

            tutorials[14].GetComponentInChildren<Text>().text = "para usar o  \"dash\" pressiona na tecla SPACE ou no botao direito do rato enquanto apontas com o rato na direccao que queres ir!  Tenta agora ...";

            tutorials[15].GetComponentInChildren<Text>().text = "Estas pronto para comecar! Lembra-te que o \"Stash\" nao consegue transportar mais do que 4 reliquias. Larga as reliquias no altar para libertar espaco!";
            tutorials[15].transform.GetChild(1).GetComponentInChildren<Text>().text = "Percebido!";

            tutorials[16].GetComponentInChildren<Text>().text = "Adeus viajante. Estou a contar contigo! Nao me decepciones!";
            tutorials[16].transform.GetChild(1).GetComponentInChildren<Text>().text = "Adeus!";
        }
    }

    void RefreshTutorialTexts()
    {
        foreach(GameObject tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }
        tutorials[tutorialStage].SetActive(true);
    }

	void Update () {
        //Check if the tutorial has ended
        if (tutorialStage >= tutorials.Length)
        {
            prophetBaloon.SetActive(false);

        }
        else if(tutorialStage == 6)
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                this.GetComponent<Beacon>().SpreadInitialGoldenMessages();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                usedW = true;
                TextW.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                usedA = true;
                TextA.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                usedS = true;
                TextS.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                usedD = true;
                TextD.color = Color.red;
            }

            if (usedW && usedA && usedS && usedD)
            {
                NextTutorial();
            }
        }
        else if (tutorialStage == 7 && Input.GetMouseButtonDown(0))
        {
            NextTutorial();
        }
        else if (tutorialStage == 14 && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)))
        {
            NextTutorial();
        }
    }
}

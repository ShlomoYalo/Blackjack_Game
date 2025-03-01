using System;
using System.Timers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

//TODO spilt, double, cancel bet buttons, timer, exit button, database, AI,

public class GameManager : MonoBehaviour
{
    //Game buttons
    public Button dealBtn, hitBtn, standBtn; 
    public Button bet10Btn, bet50Btn, bet100Btn, bet200Btn;
    public Text scoreText, dealerScoreText,mainText;
    public GameObject hideCard;

    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    public BetManager betManager;
    int pot = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        // Add on click listeners to the buttons
        dealBtn.onClick.AddListener(DealClicked);
        hitBtn.onClick.AddListener(HitClicked);
        standBtn.onClick.AddListener(StandClicked);

        bet10Btn.onClick.AddListener(() => betManager.PlaceBet(10));
        bet50Btn.onClick.AddListener(() => betManager.PlaceBet(50));
        bet100Btn.onClick.AddListener(() => betManager.PlaceBet(100));
        bet200Btn.onClick.AddListener(() => betManager.PlaceBet(200));

        // Add a clear button - if the user wont to reset the bet
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
    }

    private void DealClicked()
    {
        // TODO check if the player put mouny before start the game
        
        playerScript.ResetHand();
        dealerScript.ResetHand();
        mainText.gameObject.SetActive(false);
        // Hide the dealer score at the start of the deal
        dealerScoreText.gameObject.SetActive(false);

        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StarHand();
        dealerScript.StarHand();
        // Update the score display
        UpdateScoreUI();
        // Enable to hide one of the dealer's cards
        hideCard.GetComponent<Renderer>().enabled = true;
        ToggleGameButtons(true);

        betManager.DoublePot();
        RoundOver("player");
    }

    private void HitClicked()
    {
        playerScript.GetCard();
        UpdateScoreUI();
        RoundOver("player");
    }
    
    private void StandClicked()
    {
        DealerTurn();
    }

    private void DealerTurn()
    {
        hideCard.GetComponent<Renderer>().enabled = false;

        if(dealerScript.handValue > playerScript.handValue)
        {
            RoundOver("dealerHit");
        }
        else
        {
            HitDealer();

        }
    }

    private void HitDealer()
    {
        // TODO show the cards one by one 
        while (dealerScript.handValue < 17)
        {
            dealerScript.GetCard();
            UpdateScoreUI();
            RoundOver("dealerHit");

        }
        RoundOver("dealerHit");
    }

    private void RoundOver(String name)
    {
        bool roundOver = true;

        // if the player bust/win after deal/hit button
        if (name == "player")
        {
            if (playerScript.handValue > 21)
            {
                mainText.text = "Bust: Dealer Win";
            }
            else if (playerScript.handValue == 21)
            {
                mainText.text = "Blackjack!! You Win";
                playerScript.AdjustMoney(pot);
            }
            else
            {
                roundOver = false;
            }
        } 

        else if (name == "dealerHit")
        {
            if (dealerScript.handValue == 21)
            {
                mainText.text = "Dealer Win!! Blackjack";

            }

            else if (dealerScript.handValue > 21)
            {
                mainText.text = "Dealer Bust: You Win - Dealer Hand: " + dealerScript.handValue.ToString();
                playerScript.AdjustMoney(pot);

            }

            else if (dealerScript.handValue > playerScript.handValue)
            {
                mainText.text = "Dealer Wins - Dealer Hand: " + dealerScript.handValue.ToString();
                
            }

            else if (playerScript.handValue > dealerScript.handValue && dealerScript.handValue >= 17)
            {
                mainText.text = "You Win!! - Dealer Hand: " + dealerScript.handValue.ToString(); 
                playerScript.AdjustMoney(pot);

            }

            else if (playerScript.handValue == dealerScript.handValue && dealerScript.handValue >= 17)
            {
                mainText.text = "Push: Bets returned";
                playerScript.AdjustMoney(pot/2);
            }
            else
            {
                roundOver = false;
            }
        }

        if (roundOver)
        {
            ResetRound();
            pot = 0;
        }
    }

    private void ResetRound()
    {
        ToggleGameButtons(false);
        mainText.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(true);
        hideCard.GetComponent<Renderer>().enabled = false;
        betManager.ResetBet();
        
    }


    private void UpdateScoreUI()
    {
        scoreText.text = "HAND: " + playerScript.handValue;
        dealerScoreText.text = "HAND: " + dealerScript.handValue;
    }

    private void ToggleGameButtons(bool enabled)
    {
        dealBtn.gameObject.SetActive(!enabled);
        hitBtn.gameObject.SetActive(enabled);
        standBtn.gameObject.SetActive(enabled);

        bet10Btn.enabled = !enabled;
        bet50Btn.enabled = !enabled;
        bet100Btn.enabled = !enabled;
        bet200Btn.enabled = !enabled;
    }

    
}

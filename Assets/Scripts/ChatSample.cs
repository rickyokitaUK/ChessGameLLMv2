using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Gpt4All.Samples
{
    public class ChatSample : MonoBehaviour
    {
        public LlmManager manager;

        [Header("UI")]
        public InputField input;
        public Text output;
        public Button submit;

        private string _previousText;
        private List<string> chatHistory = new List<string>(); // Store chat history here

        

        private void Awake()
        {
            input.onEndEdit.AddListener(OnSubmit);
            submit.onClick.AddListener(OnSubmitPressed);
            manager.OnResponseUpdated += OnResponseHandler;
        }

        private void OnSubmit(string prompt)
        {
            if (!Input.GetKey(KeyCode.Return))
                return;
            SendToChat(input.text);
        }

        private void OnSubmitPressed()
        {
            SendToChat(input.text);
        }

        private char MapIntToLetter(int number)
        {
            if (number >= 1 && number <= 8)
            {
                // Subtract 1 from the input number to get the correct index in the range 0 to 7.
                int index = number - 1;

                // Use 'a' as the starting point and add the index to get the corresponding letter.
                char letter = (char)('a' + index);

                return letter;
            }
            else
            {
                // Handle invalid input (numbers outside the range 1-8) or return a default value.
                throw new System.ArgumentException("Input number must be between 1 and 8.");
            }
        }

        public void GetChessCoordinate(bool chessIsWhite, System.Type chesstype, int x, int y)
        {
            string chessInput = "";

            Debug.Log("Get Chess Coordinate : " + x + "," + y + " | Chess Type: " + chesstype);

            char ctype;
           
            switch (chesstype)
            {
                case Type t when t == typeof(King):
                    ctype = 'k';
                    break;
                case Type t when t == typeof(Queen):
                    ctype = 'q';
                    break;
                case Type t when t == typeof(Bishop):
                    ctype = 'b';
                    break;
                case Type t when t == typeof(Rook):
                    ctype = 'r';
                    break;
                case Type t when t == typeof(Pawn):
                    ctype = 'p';
                    break;
                case Type t when t == typeof(Knight):
                    ctype = 'n';
                    break;
                default:
                    ctype = 'P'; // Default value if chesstype doesn't match any of the known types.
                    break;
            }
            if (!chessIsWhite)
            {
                // Handle the case when the chess piece is not white, e.g., set ctype to lowercase.
                // You can define the behavior you want for non-white pieces here.
                ctype = char.ToLower(ctype); // For example, set to lowercase.
            }


            chessInput = ctype.ToString() +  MapIntToLetter(x).ToString() +  y.ToString();
            SendToChat(chessInput);

            Debug.Log("ChessInput: " + chessInput);


        }

        private async void SendToChat(string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
                return;

            input.text = "";
            string userMessage = $"<b>User:</b> {prompt}\n<b>Answer:</b> ";
            output.text += userMessage;
            _previousText = userMessage;

            await manager.Prompt(prompt);

            // Add the user message to the chat history
            chatHistory.Add(userMessage);

            output.text += "\n";
        }

        private void OnResponseHandler(string response)
        {
            output.text = _previousText + response;

            // Add the AI response to the chat history
            chatHistory.Add(response);
        }
    }
}
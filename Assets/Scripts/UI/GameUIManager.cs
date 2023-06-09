﻿using System;
using UnityEngine;

namespace Campaigner.UI
{
    
    public class GameUIManager : MonoBehaviour
    {
        
        public static GameUIManager Instance { get; private set; }
        
        public event Action<GameMenuState> OnGameMenuStateChanged;
        
        public bool CanSwitchMenuState { get; set; } = true;

        public event Action<bool> OnGamePausedChanged;
        
        private bool _gamePaused;
        
        public bool GamePaused
        {
            get => _gamePaused;
            set
            {
                _gamePaused = value;
                OnGamePausedChanged?.Invoke(_gamePaused);
            }
        }
        
        
        private GameMenuState _gameMenuState;
        public GameMenuState GameMenuState
        {
            get => _gameMenuState;
            set
            {
                _gameMenuState = value;
                OnGameMenuStateChanged?.Invoke(_gameMenuState);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            HandleInput();
        }
    
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapeHandler();
            }
        }

        /// <summary>
        /// Sets the GameMenuState based on the current GameMenuState
        /// </summary>
        private void EscapeHandler()
        {
            if (!CanSwitchMenuState)
            {
                return;
            }
            
            switch (GameMenuState)
            {
                case GameMenuState.BaseView:
                    GamePaused = true;
                    GameMenuState = GameMenuState.EscMenu;
                    break;
                
                case GameMenuState.EscMenu:
                    GamePaused = false;
                    GameMenuState = GameMenuState.BaseView;
                    break;
                
                case GameMenuState.SaveMenu:
                    GameMenuState = GameMenuState.EscMenu;
                    break;
                
                case GameMenuState.LoadMenu:
                    GameMenuState = GameMenuState.EscMenu;
                    break;
                
                case GameMenuState.SettingsMenu:
                    GameMenuState = GameMenuState.EscMenu;
                    break;
                
                case GameMenuState.QuitMenu:
                    GameMenuState = GameMenuState.EscMenu;
                    break;
                
                case GameMenuState.PlaceObjectUI:
                    GameMenuState = GameMenuState.BaseView;
                    break;
            }
        }
        
        public void BackToMainMenu()
        {
            GameMenuState = GameMenuState.BaseView;
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        }

    }
}
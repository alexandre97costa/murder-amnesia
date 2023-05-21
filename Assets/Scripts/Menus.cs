using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    [SerializeField] private string nivelJogo;
    [SerializeField] private GameObject painelMenuPrincipal;
    [SerializeField] private GameObject painelDefinicoes;
    [SerializeField] private GameObject painelCreditos;
    [SerializeField] private GameObject painelVideo;
    [SerializeField] private GameObject painelSom;
    [SerializeField] private GameObject painelControlos;

    public void Iniciar()
    {
        SceneManager.LoadScene(nivelJogo);
    }


    public void AbrirDefinicoes()
    {
        painelDefinicoes.SetActive(true);
        painelMenuPrincipal.SetActive(false);
        painelCreditos.SetActive(false);
        painelVideo.SetActive(false);
        painelSom.SetActive(false);
        painelControlos.SetActive(false);
    }

    public void FecharDefinicoes()
    {
        painelMenuPrincipal.SetActive(true);
        painelDefinicoes.SetActive(false);
        painelCreditos.SetActive(false);
        painelVideo.SetActive(false);
        painelSom.SetActive(false);
        painelControlos.SetActive(false);
    }

    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);
        painelMenuPrincipal.SetActive(false);
        painelDefinicoes.SetActive(false);
        painelVideo.SetActive(false);
        painelSom.SetActive(false);
        painelControlos.SetActive(false);
    }

    public void FecharCreditos()
    {
        AbrirDefinicoes();
    }

    public void AbrirVideo()
    {
        painelVideo.SetActive(true); 
        painelCreditos.SetActive(false);
        painelMenuPrincipal.SetActive(false);
        painelDefinicoes.SetActive(false);
        painelSom.SetActive(false);
        painelControlos.SetActive(false);
    }

    public void FecharVideo()
    {
        AbrirDefinicoes();
    }

    public void AbrirSom()
    {
        painelSom.SetActive(true);
        painelCreditos.SetActive(false);
        painelMenuPrincipal.SetActive(false);
        painelDefinicoes.SetActive(false);
        painelVideo.SetActive(false);
        painelControlos.SetActive(false);
    }

    public void FecharSom()
    {
        AbrirDefinicoes();
    }

    public void AbrirControlos()
    {
        painelControlos.SetActive(true);
        painelCreditos.SetActive(false);
        painelMenuPrincipal.SetActive(false);
        painelDefinicoes.SetActive(false);
        painelSom.SetActive(false);
        painelVideo.SetActive(false);
    }

    public void FecharControlos()
    {
        AbrirDefinicoes();
    }


    public void Sair()
    {
        Debug.Log("Sair do Jogo");
        Application.Quit();
    }
}

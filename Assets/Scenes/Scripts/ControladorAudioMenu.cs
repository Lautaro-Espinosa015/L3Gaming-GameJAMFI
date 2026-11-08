using UnityEngine;

public class ControladorAudioMenu : MonoBehaviour
{
    public AudioSource musicaMenu;

    [Header("Duración de la Animación")]
    public float tiempoAnimacionCarro = 5f; // Segundos que tarda el carro en llegar

    void Awake()
    {
        AudioListener.pause = true;
    }

    void Start()
    {
        if (musicaMenu != null)
        {
            musicaMenu.ignoreListenerPause = true;
            if (!musicaMenu.isPlaying) musicaMenu.Play();
        }
    }

    public void ActivarSonidosJuego()
    {
        // 1. Apagamos la música del menú instantáneamente
        if (musicaMenu != null) musicaMenu.Stop();

        // 2. Iniciamos una cuenta regresiva para encender el mundo
        Invoke("QuitarTaponesMundo", tiempoAnimacionCarro);
    }

    private void QuitarTaponesMundo()
    {
        // 3. ¡Terminó el tiempo! El mundo vuelve a sonar
        AudioListener.pause = false;
    }
}
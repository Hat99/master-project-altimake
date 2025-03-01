using UnityEngine;



/************************************/
/*"thread" for handling timed events*/
/************************************/

public class TimedThread : MonoBehaviour
{
    #region fields

    //interval for auto saving
    public float autoSaveInterval = 60f;
    private float autoSaveTimer;

    //interval for refreshing the file view
    public float refreshFileView = 1f;
    private float refreshFileViewTimer;

    public bool imageLoaded;
    private int imageLoadingIndex;
    private bool loadingImages;

    //allows calling from other classes
    public static TimedThread instance;

    #endregion fields



    #region methods

    void Start()
    {
        instance = this;

        //freeze time until it is needed
        UseTime(false);

        //initiate auto save timer
        autoSaveTimer = autoSaveInterval;

        //initiate file view refresh timer
        //(close to 0 to see files asap but not 0 because updates are still called)
        refreshFileViewTimer = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        //auto save
        autoSaveTimer -= Time.deltaTime;
        if(autoSaveTimer < 0)
        {
            //TODO: if saving takes noticable time, show loading icon?
            AltimateHelper.SaveAltimate();
            autoSaveTimer = autoSaveInterval;
        }

        //refresh file view
        refreshFileViewTimer -= Time.deltaTime;
        if (refreshFileViewTimer < 0)
        {
            MainMenuHandler.instance.RefreshFileView();
            refreshFileViewTimer = refreshFileView;
        }

        //load images one by one to not freeze the main thread too long at a time
        //TODO: show loading progress and prohibit inputs until loading has finished
        if (loadingImages && imageLoaded)
        {
            imageLoaded = false;
            imageLoadingIndex--;
            if(imageLoadingIndex >= 0)
            {
                ImageHandler.instance.LoadImage(AltimateHelper.images[imageLoadingIndex]);
            }
            else
            {
                loadingImages = false;
            }
        }
    }

    //toggle time on and off
    public static void UseTime(bool val)
    {
        if (val)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    public void InitializeImageLoading()
    {
        //load images in reverse order to build from background to foreground
        imageLoadingIndex = AltimateHelper.images.Count - 1;
        if (imageLoadingIndex >= 0) 
        {
            loadingImages = true;
            imageLoaded = false;
            ImageHandler.instance.LoadImage(AltimateHelper.images[imageLoadingIndex]);
        }
    }

    #endregion methods
}

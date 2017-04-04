using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(SpriteRenderer))]
public class MarqueeLayerPanel : MonoBehaviour {
    MaterialPropertyBlock MatProp;
    public float FadeDuration = 1.0f;
    protected SpriteRenderer ImageRender;
    protected VideoPlayer VidPlayer = null;
    protected bool Prepared = false;

    public string ResourcePath = "";
    public enum MarqueeResourceType {
        Image,
        Video,
        WebVideo,
        Internal
    } public MarqueeResourceType ResourceType = MarqueeResourceType.Image;

    void SetupVideoPlayer() {
        if (VidPlayer == null)
            VidPlayer = gameObject.AddComponent<VideoPlayer>();

        VidPlayer.playOnAwake = true;
        VidPlayer.waitForFirstFrame = true;
        VidPlayer.url = ResourcePath;
        VidPlayer.targetMaterialRenderer = ImageRender;
    }
    
	void Start () {
        MatProp = new MaterialPropertyBlock();
        ImageRender = GetComponent<SpriteRenderer>();

        switch (ResourceType) {
            default:
            case MarqueeResourceType.Image:
                break;
            case MarqueeResourceType.Video: 
                SetupVideoPlayer();
                try {
                    VidPlayer.clip = Resources.Load<VideoClip>(ResourcePath);
                } catch(UnityException e) {
                    Debug.LogError(e.Message);
                    Debug.LogWarning("Destroying " + name);
                    Destroy(gameObject);
                }

                VidPlayer.prepareCompleted += VideoPrepared;
                VidPlayer.Prepare();
                break;
            case MarqueeResourceType.WebVideo: 
                SetupVideoPlayer();
                VidPlayer.url = ResourcePath;
                VidPlayer.prepareCompleted += VideoPrepared;
                VidPlayer.Prepare();
                break;
            case MarqueeResourceType.Internal:
                try {
                    ImageRender.sprite = Resources.Load<Sprite>(ResourcePath);
                } catch (UnityException e) {
                    Debug.LogError(e.Message);
                    Debug.LogWarning("Destroying " + name);
                    Destroy(gameObject);
                }
                break;
        }

        gameObject.SetActive(false);
    }

    private void VideoPrepared(VideoPlayer source) {
        Prepared = true;
        if (VidPlayer != source)
            Debug.LogError("Prepared video does not match source!");
        VidPlayer = source;
    }

    public void Fade(bool FadeIn) {

        //Setup material for fade
        MatProp.SetFloat("_Alpha", FadeIn ? 0.0f : 1.0f);
        ImageRender.SetPropertyBlock(MatProp);

        //Show Object
        gameObject.SetActive(true);

        switch (ResourceType) {
            default:
            case MarqueeResourceType.Image:
                break;
            case MarqueeResourceType.Video:
            case MarqueeResourceType.WebVideo:
            case MarqueeResourceType.Internal:
                //Start Playing
                SendMessage("Play", gameObject, SendMessageOptions.DontRequireReceiver);
                break;
        }

        iTween.FadeTo(gameObject, FadeIn ? 1.0f : 0.0f, FadeDuration);
    }
}

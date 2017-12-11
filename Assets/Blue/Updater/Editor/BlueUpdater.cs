using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MiniJSON;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class BlueUpdater
{
    const string GITHUB_API = "https://api.github.com/repos/{0}/{1}/releases";
    private readonly string releasesJSON;

    public BlueUpdater(string username, string repoName)
    {
        releasesJSON = GetReleases(username, repoName);
    }

    private WebClient webClient()
    {
        WebClient webClient = new WebClient();
        webClient.Headers.Add("User-Agent", "Unity web player");
        return webClient;
    }

    public void DownloadPackage()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        Dictionary<string, object> assets = GetAssets();
        const string FileName = "package.unityPackage";
        if (DownloadFile(assets["browser_download_url"] as string, FileName))
        {
            AssetDatabase.ImportPackage(FileName, true);
        }
    }

    public bool IsNewVersionAvailable(string currentVersion)
    {
        string latestVersion = GetLatestReleaseVersion();

        Version current = new Version(currentVersion);
        Version latest = new Version(latestVersion);

        var result = current.CompareTo(latest);

        return result < 0;
    }

    private string GetLatestReleaseVersion()
    {
        List<object> releasesList = Json.Deserialize(releasesJSON) as List<object>;
        Dictionary<string, object> firstIndex = releasesList[0] as Dictionary<string, object>;
        return firstIndex["tag_name"] as string;
    }

    private Dictionary<string, object> GetAssets()
    {
        List<object> releasesList = Json.Deserialize(releasesJSON) as List<object>;
        Dictionary<string, object> firstIndex = releasesList[0] as Dictionary<string, object>;
        List<object> assets = firstIndex["assets"] as List<object>;
        Dictionary<string, object> assetsDictionary = assets[0] as Dictionary<string, object>;
        return assetsDictionary;
    }

    private string GetReleases(string username, string repoName)
    {
        WebClient wc = webClient();
        Uri uri = new Uri(string.Format(GITHUB_API, username, repoName));
        string releases = wc.DownloadString(uri);
        return releases;
    }

    private bool DownloadFile(string url, string fileName)
    {
        WebClient wc = webClient();
        Uri uri = new Uri(url);
        try
        {
            wc.DownloadFile(uri, fileName);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return false;
    }

    public bool MyRemoteCertificateValidationCallback(Object sender, X509Certificate certificate,
        X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2) certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
}
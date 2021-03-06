﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using NowMine.Helpers;
using NowMine.Queue;

namespace NowMine
{
    class SearchPanel
    {
        YouTubeProvider youtubeProvider = new YouTubeProvider();
        StackPanel stackPanel;
        TextBox textBox;

        public delegate void VideoQueuedEventHandler(object s, MusicPieceReceivedEventArgs e);
        public SearchPanel(StackPanel stackPanel, TextBox textBox)
        {
            this.stackPanel = stackPanel;
            this.textBox = textBox;
        }

        public void search()
        {
            if (textBox.Text == "")
            {
                return;
            }
            List<MusicPiece> list;
            list = getSearchList(textBox.Text);
            populateSearchBoard(list);
        }

        private void populateSearchBoard(List<MusicPiece> results)
        {
            stackPanel.Children.Clear();
            foreach (MusicPiece result in results)
            {
                result.MouseDoubleClick += SearchResult_MouseDoubleClick;
                stackPanel.Children.Add(result);
            }
        }

        private void SearchResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            var queueMusicPiece = musicPiece.copy();
            queueMusicPiece.userColorBrush();
            queueMusicPiece.lbluserName.Visibility = System.Windows.Visibility.Visible;
            int qPos = QueueManager.addToQueue(queueMusicPiece);
        }

        public List<MusicPiece> getSearchList(String searchWord)
        {
            List<MusicPiece> list;
            List<YouTubeInfo> infoList = youtubeProvider.LoadVideosKey(searchWord);
            list = infoToResults(infoList);
            return list;
        }

        private List<MusicPiece> infoToResults(List<YouTubeInfo> infoList)
        {
            List<MusicPiece> list = new List<MusicPiece>();
            foreach (YouTubeInfo info in infoList)
            {
                MusicPiece result = new MusicPiece(info);
                list.Add(result);
            }
            return list;
        }
    }
}

using System;

namespace DesignLibrary_Tutorial
{
    public class DummyModel
    {

        private long mId;
        private String mImageURL;
        private String mText;
        private int mIconRes;

        public DummyModel()
        {

        }

        public DummyModel(long id, String imageURL, String text, int iconRes)
        {
            mId = id;
            mImageURL = imageURL;
            mText = text;
            mIconRes = iconRes;
        }

        public long getId()
        {
            return mId;
        }

        public void setId(long id)
        {
            mId = id;
        }

        public String getImageURL()
        {
            return mImageURL;
        }

        public void setImageURL(String imageURL)
        {
            mImageURL = imageURL;
        }

        public String getText()
        {
            return mText;
        }

        public void setText(String text)
        {
            mText = text;
        }

        public int getIconRes()
        {
            return mIconRes;
        }

        public void setIconRes(int iconRes)
        {
            mIconRes = iconRes;
        }


        public override string ToString()
        {
            return mText;
        }
    }

}
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using System.IO;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace DesignLibrary_Tutorial.Fragments
{
    public class Fragment2 : SupportFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                if (resultCode == (int)Android.App.Result.Ok)
                {
                    //no choose no continue to here
                    //Uri uri = data.Data;
                    //get file's url , so last can get file
                    //string[] proj = { MediaStore.Images.Media.Query };
                    //Cursor actualimagecursor = MediaStore.Images.Media.Query(uri, proj, null, null, null);
                    //int actual_image_column_index = actualimagecursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
                    //actualimagecursor.moveToFirst();
                    //String img_path = actualimagecursor.getString(actual_image_column_index);
                    //File file = new File(img_path);
                    //Toast.makeText(MainActivity.this, file.toString(), Toast.LENGTH_SHORT).show();
                    Uri uri = data.Data;
                    // _imageView.SetImageURI(uri);
                    PS4_Tools.Tools.File_Type typeoffile = PS4_Tools.Tools.Get_PS4_File_Type(uri.Path.ToString());
                   
                }
            }
             
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Fragment2, container, false);

            Button btnLogin = view.FindViewById<Button>(Resource.Id.btnLogin);
            TextInputLayout passwordWrapper = view.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutPassword);
            string txtPassword = passwordWrapper.EditText.Text;

            btnLogin.Click += (o, e) =>
            {
                Intent intent = new Intent(Intent.ActionGetContent);
                intent.SetType("*/*");//set type , here is setted to every type.
                intent.AddCategory(Intent.CategoryOpenable);
                StartActivityForResult(intent, 1);

                if (txtPassword != "1234")
                {
                    passwordWrapper.Error = "Wrong password, try again";
                }
            };

            return view;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniGame
{
    public class RegisterPage : UIBase
    {
        public static RegisterPage instance;
        public static RegisterPage Open(System.Action<RegisterData> done) => Open(null, done);
        public static RegisterPage Open(RegisterData oldData, System.Action<RegisterData>  done )
        {
            instance = CreatePage<RegisterPage>(GameStore.instance.page.prefab_registerPage);
            instance.Init(oldData,done);
            return instance;
        }





        RegisterData registerData;
        public class RegisterData 
        {
            public string name;
            public string lastname;
            public string nickname;
        }


        public UILabel ui_lbHeder;
        public UIInput input_name;
        public UIInput input_lastname;
        public UIInput input_nicekname;

        System.Action<RegisterData> m_done;
        public void Init(RegisterData oldData , System.Action<RegisterData> done) 
        {
            m_done = done;

            if (oldData == null)
            {
                ui_lbHeder.text = "Register";
                registerData = new RegisterData();
            }
            else 
            {
                ui_lbHeder.text = "Edit Profile";
                registerData = oldData;
                input_name.value = registerData.name;
                input_lastname.value = registerData.lastname;
                input_nicekname.value = registerData.nickname;
            }
          
        }
        public void ClosePage()
        {
            OnClose();
        }
        public void Done()
        {

            if (input_name.value.isnull())
            {
                input_name.label.color = Color.red;
                return;
            }
            if (input_lastname.value.isnull())
            {
                input_lastname.label.color = Color.red;
                return;
            }
            if (input_nicekname.value.isnull())
            {
                input_nicekname.label.color = Color.red;
                return;
            }



            registerData.name = input_name.value;
            registerData.lastname = input_lastname.value;
            registerData.nickname = input_nicekname.value;
            m_done?.Invoke(registerData);
            OnClose();
        }


    }
}
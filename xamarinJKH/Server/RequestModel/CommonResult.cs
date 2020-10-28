namespace xamarinJKH.Server.RequestModel
{
    public class CommonResult
    {
        public string Error { get; set; }
    }
    
    public class AddAccountResult : CommonResult
    {
        public string Address { get; set; } 	// Заполняется в режиме Confirm=true 
                                                //если ЛС может быть добавлен
        public string acx { get; set; } 	//  токен доступа

    }
}
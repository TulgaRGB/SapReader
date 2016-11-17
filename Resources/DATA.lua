this.Name = "Смена данных"
this.Author = "LISENKO SOFT"
this:Draw([[
<FORM>
<label location="12,12" font="12" >Поменять имя</label> 
<label location="222,42" font="10" >Новое имя</label> 

<textbox location="12,42" size="200,20" name="newName"></textbox> 

<label location="12,72" font="12" >Поменять пароль</label> 

<label location="222,102" font="10" >Старый пароль</label> 
<textbox location="12,102" size="200,20" name="oldPass" secured=""></textbox> 
<label location="222,132" font="10" >Новый пароль</label> 
<textbox location="12,132" size="200,20" name="newPass" secured=""></textbox> 
<label location="222,162" font="10" >Повтор пароля</label> 
<textbox location="12,162" size="200,20" name="checkPass" secured=""></textbox> 

<button location="12,192" name="save">Сохранить изменения</button> 

<label location="12,230" font="7" name="error"></label> 
</FORM>
]])

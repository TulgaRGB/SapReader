this.Name = "News"
this.Author = "LISENKO SOFT"

this:Draw([[
<FORM> 
<label location = "12,12" font="18" name="header">Заголовок новости</label> 
<label location = "12,40" font="8" name="time"></label> 
<label location = "12,60" font="12" name="article" size="700,150"></label> 
<textbox name="commentInput" location="12,215" size="500,23"></textbox> 
<button name="inputButton" location="513,215" size="99,23">Отправить</button>
</FORM>
]])

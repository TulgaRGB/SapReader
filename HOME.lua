labelStart = "Для начала работы воспользуйтесь формой ниже или используйте меню в верхней части программы."
labelAbout = "Программа предназначена для работы в двух режимах: проводник с возможностью шифровать файлы и архиватор зашифрованных архивов."
labelSap = [[Security Algorythm, Private Protection From InfoRmation Exposure (Sapphire™) Защитный алгоритм, частная защита от уязвимости 
информации - криптографический алгоритм с закрытым ключом для хранения и компакной передачи информации.]]
this.Name = "Добро пожаловать!"
this.Author = "LISENKO SOFT"

this:Draw([[
<FORM> 
<label location="12,12" font="18">SapReader</label> 
<button location="12,50" name="clickStart">Начало работы</button> 
<button location="112,50" name="clickAbout">О программе</button> 
<button location="203,50" name="clickSap">О Sapphire™</button> 

<label location="12,80" name="mainLabel">Добро пожаловать в SapReader.</label>

<textbox location="12,117" size="420,20" name="key">Введите ключ</textbox> 
<richtextbox location="12,139" size="420,134" name="text">Введите текст</richtextbox> 
<button location="12,275" name="enc">Шифровать</button> 
<button location="100,275" name="dec">Расшифровать</button> 



<label location="12,300" font="7">SapphireReader powered by Sapphire™
LISENKO SOFT 2016 год
</label>
</FORM>
]])

function handleClick(sender,data)
if sender.Name == "clickStart" then
  Controls.mainLabel.Text = labelStart
  elseif sender.Name == "clickAbout" then
  Controls.mainLabel.Text = labelAbout
  else
  Controls.mainLabel.Text = labelSap
  end  
  end

Controls.clickStart.MouseUp:Add(handleClick)
Controls.clickAbout.MouseUp:Add(handleClick)
Controls.clickSap.MouseUp:Add(handleClick)

function sapClick(sender,data)
if sender.Name == "enc" then
Controls.text.Text = Sapphire.GetCode(Controls.text.Text, Controls.key.Text)
else
Controls.text.Text = Sapphire.GetText(Controls.text.Text, Controls.key.Text)
end
end
Controls.enc.MouseUp:Add(sapClick)
Controls.dec.MouseUp:Add(sapClick)
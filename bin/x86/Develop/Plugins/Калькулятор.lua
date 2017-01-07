this:Info([[<INFO name="Калькулятор" author="fast" window="True"/>]])
--Form initial
main = this:AddWindow()
main:Draw([[<FORM>
  <textbox name="from" location="12,12" size="289,50" font="24" readonly="">0</textbox>
  <button location="12,60" size="73" font="36" name="b1">1</button>
  <button location="84,60" size="73" font="36" name="b2">2</button>
  <button location="156,60" size="73" font="36" name="b3">3</button>
  <button location="12,132" size="73" font="36" name="b4">4</button>
  <button location="84,132" size="73" font="36" name="b5">5</button>
  <button location="156,132" size="73" font="36" name="b6">6</button>
  <button location="12,204" size="73" font="36" name="b7">7</button>
  <button location="84,204" size="73" font="36" name="b8">8</button>
  <button location="156,204" size="73" font="36" name="b9">9</button>
  <button location="228,60" size="73" font="36" name="b10">+</button>
  <button location="228,132" size="73" font="36" name="b11">-</button>
  <button location="228,204" size="73" font="36" name="b12">*</button>
  <button location="228,276" size="73" font="36" name="b13">/</button>
  <button location="12,276" size="73" font="36" name="bcl">C</button>
  <button location="84,276" size="73" font="36" name="b0">0</button>
  <button location="156,276" size="73" font="36" name="bdo">=</button>
</FORM>]])
main.TopMost = true
main.Width = 313
main.Height = 362
LSFB.AddLSFB(main,2,0,0,false)
main.Text = this.Name
main:Show()
for i = 0,13 do
main.WindowControls["b"..i].MouseUp:Add(NumberClick)
end
--Handle
clear = true
function NumberClick(sender, data)
if clear == true then
main.WindowControls["from"].Text = ""
else
main.WindowControls["from"].Text = main.WindowControls["from"].Text .. sender.Text
end
clear = false
end
Info([[<INFO name="TickTackToe" author="fast" window="false"/>]])
X = true
Draw([[<FORM>
  <label location="12,12" font="24" name="turn"></label>
  <button location="12,60" size="73" font="36" name="b1"></button>
  <button location="84,60" size="73" font="36" name="b2"></button>
  <button location="156,60" size="73" font="36" name="b3"></button>
  <button location="12,132" size="73" font="36" name="b4"></button>
  <button location="84,132" size="73" font="36" name="b5"></button>
  <button location="156,132" size="73" font="36" name="b6"></button>
  <button location="12,204" size="73" font="36" name="b7"></button>
  <button location="84,204" size="73" font="36" name="b8"></button>
  <button location="156,204" size="73" font="36" name="b9"></button>
</FORM>]])
hod = "Ход игорка "
Controls.turn.Text = hod .. "X"
function Check()
end
function Click(sender, data)
for i = 1 , 9 do
if(sender.Name == "b" .. i) then
if X == true then
sender.Text = "X"
Controls.turn.Text = hod .. "O"
else
sender.Text = "O"
Controls.turn.Text = hod .. "X"
end
X = not X
Check()
end
end
end
for i = 1, 9 do
Controls["b"..i].MouseUp:Add(Click)
end

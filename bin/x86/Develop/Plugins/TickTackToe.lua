this:Info([[<INFO name="TickTackToe" author="fast" window="false" sign="
6a7CzHcieUnQg/2GICN6xD28m+Sgx4Q6iYq4x4eA+L/qSDKZlR
nD87D6rc5ElCOaHER7fsxXaOjAS6mwznco6h4IBXVNePXrkUDK
cLJNjMDN4wwGnBNeWmsTQN8M2LYDXVxBByh32W/kVq9TUpTPOI
arebT+7OLVF3KZBZ+vplEiX9UYnhs0IJtht98nz5qkCM3o9/CZ
kwvgBIVOo280+XT3l4xRUfsD4PKAN8XKQAxtjY6XKEa9SitinG
IlqQz2v1UAx5TXuE45r33KhSjW3ScUqKw9aWbFmxn1uRpnRf9y
UQ0+LEXWLD4N9kFgv13MIYWa2pgsRu+CApFIdsXIlg=="/>]])
X = true
this:Draw([[<FORM>
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
xwin = false
owin = false
for i = 1,3 do 
if Controls["b" .. i].Text ==  Controls["b" .. i + 3].Text and  Controls["b" .. i + 3].Text ==  Controls["b" .. i + 6].Text and Controls["b" .. i].Text ~= "" then
Win(Controls["b" .. i].Text )
end
end
for i = 0,2 do 
if Controls["b" .. i * 3 + 1].Text ==  Controls["b" .. i * 3 + 2].Text and  Controls["b" .. i * 3 + 2].Text ==  Controls["b" .. i * 3 + 3].Text and Controls["b" .. i * 3+1].Text ~= "" then
Win(Controls["b" .. i * 3 + 1].Text )
end 
end
if Controls.b1.Text == Controls.b5.Text and Controls.b5.Text == Controls.b9.Text  and Controls.b1.Text ~= "" then
Win(Controls.b1.Text )
end
if Controls.b3.Text == Controls.b5.Text and Controls.b5.Text == Controls.b7.Text  and Controls.b3.Text ~= "" then
Win(Controls.b3.Text )
end
jopa = true
for i = 1,9 do 
if Controls["b" .. i].Text == "" then
jopa = false
end 
end
if jopa then
Win()
end
end

function Win(who)
if who == nil then
this:Alert("Ничья")
else
this:Alert(who.. " выйграл!")
end
for i = 1, 9 do
Controls["b"..i].Text = ""
end
end

function Click(sender, data)
if sender.Text == "" then
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
end

for i = 1, 9 do
Controls["b"..i].MouseUp:Add(Click)
end
labelStart = "��� ������ ������ �������������� ������ ���� ��� ����������� ���� � ������� ����� ���������."
labelAbout = "��������� ������������� ��� ������ � ���� �������: ��������� � ������������ ��������� ����� � ��������� ������������� �������."
labelSap = [[Security Algorythm, Private Protection From InfoRmation Exposure (Sapphire�) �������� ��������, ������� ������ �� ���������� 
���������� - ����������������� �������� � �������� ������ ��� �������� � ��������� �������� ����������.]]
this.Name = "����� ����������!"
Draw([[
<FORM> 
<label location="12,12" font="18">SapReader</label> 
<button location="12,50" name="clickStart">������ ������</button> 
<button location="112,50">� ���������</button> 
<button location="203,50">� Sapphire�</button> 

<label location="12,80" name="mainLabel">����� ���������� � SapReader.</label>

<textbox location="12,117" size="420,20" name="key">������� ����</textbox> 
<richtextbox location="12,139" size="420,134" name="text">������� �����</richtextbox> 
<button location="12,275">���������</button> 
<button location="100,275">������������</button> 



<label location="12,300" font="7">SapphireReader powered by Sapphire�
LISENKO SOFT 2016 ���
</label>
</FORM>
]])

function handleClick(sender,data)
  Controls.clickStart.MouseUp:Remove(handler)
  Controls.mainLabel.Text = labelStart
end

handler=Controls.clickStart.MouseUp:Add(handleClick)
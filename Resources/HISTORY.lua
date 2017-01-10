this:Info([[<INFO name="История" author="LISENKO SOFT" window="True"/>]])

main = this:Window("main")

LSFB.AddLSFB(main, 2, 0, 0, false)
main.Text = this.Name
main.StartPosition = this.Field:FindForm().StartPosition 
main.Height = this.Field:FindForm().Height

main:Show()


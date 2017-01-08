#include "myclass.h"
#include "saveData.h"
MyClass::MyClass(QWidget *parent)
	: QMainWindow(parent)
{
	ui->setupUi(this);
}
void MyClass::on_pushButton_clicked() {
	try {
		QString discordToken = ui->tokenLineEdit->text();
		QString wolframToken = ui->wolframLineEdit->text();
		QString facebookToken = ui->facebookLineEdit->text();
		QString database = ui->databaseLineEdit->text();
		QString ownerId = ui->ownerIdLineEdit->text();

		saveData dataWriter(discordToken, facebookToken, wolframToken, ownerId, database);
		dataWriter.generateXml();

	}
	catch (int e) {
		
	}
}
MyClass::~MyClass()
{

}

#ifndef MYCLASS_H
#define MYCLASS_H

#include <QtWidgets/QMainWindow>
#include "ui_myclass.h"

class MyClass : public QMainWindow
{
	Q_OBJECT

public:
	MyClass(QWidget *parent = 0);
	~MyClass();

private slots:
	void on_pushButton_clicked();
private:
	Ui::MyClassClass *ui;
};

#endif // MYCLASS_H

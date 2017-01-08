/********************************************************************************
** Form generated from reading UI file 'myclass.ui'
**
** Created by: Qt User Interface Compiler version 5.7.1
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_MYCLASS_H
#define UI_MYCLASS_H

#include <QtCore/QVariant>
#include <QtWidgets/QAction>
#include <QtWidgets/QApplication>
#include <QtWidgets/QButtonGroup>
#include <QtWidgets/QGridLayout>
#include <QtWidgets/QHeaderView>
#include <QtWidgets/QLabel>
#include <QtWidgets/QLineEdit>
#include <QtWidgets/QMainWindow>
#include <QtWidgets/QMenu>
#include <QtWidgets/QMenuBar>
#include <QtWidgets/QPushButton>
#include <QtWidgets/QSpacerItem>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_MyClassClass
{
public:
    QAction *actionExit;
    QWidget *centralWidget;
    QGridLayout *gridLayout_2;
    QGridLayout *gridLayout;
    QLabel *label_3;
    QLabel *label_5;
    QLabel *label_2;
    QLabel *label_4;
    QPushButton *pushButton;
    QLabel *label;
    QSpacerItem *horizontalSpacer_2;
    QLineEdit *ownerIdLineEdit;
    QLineEdit *facebookLineEdit;
    QLineEdit *wolframLineEdit;
    QLineEdit *databaseLineEdit;
    QLineEdit *tokenLineEdit;
    QSpacerItem *horizontalSpacer;
    QMenuBar *menuBar;
    QMenu *menuFile;

    void setupUi(QMainWindow *MyClassClass)
    {
        if (MyClassClass->objectName().isEmpty())
            MyClassClass->setObjectName(QStringLiteral("MyClassClass"));
        MyClassClass->resize(400, 221);
        actionExit = new QAction(MyClassClass);
        actionExit->setObjectName(QStringLiteral("actionExit"));
        centralWidget = new QWidget(MyClassClass);
        centralWidget->setObjectName(QStringLiteral("centralWidget"));
        centralWidget->setMinimumSize(QSize(400, 200));
        gridLayout_2 = new QGridLayout(centralWidget);
        gridLayout_2->setSpacing(6);
        gridLayout_2->setContentsMargins(11, 11, 11, 11);
        gridLayout_2->setObjectName(QStringLiteral("gridLayout_2"));
        gridLayout = new QGridLayout();
        gridLayout->setSpacing(6);
        gridLayout->setObjectName(QStringLiteral("gridLayout"));
        label_3 = new QLabel(centralWidget);
        label_3->setObjectName(QStringLiteral("label_3"));

        gridLayout->addWidget(label_3, 2, 1, 1, 1);

        label_5 = new QLabel(centralWidget);
        label_5->setObjectName(QStringLiteral("label_5"));

        gridLayout->addWidget(label_5, 3, 1, 1, 1);

        label_2 = new QLabel(centralWidget);
        label_2->setObjectName(QStringLiteral("label_2"));

        gridLayout->addWidget(label_2, 1, 1, 1, 1);

        label_4 = new QLabel(centralWidget);
        label_4->setObjectName(QStringLiteral("label_4"));

        gridLayout->addWidget(label_4, 4, 1, 1, 1);

        pushButton = new QPushButton(centralWidget);
        pushButton->setObjectName(QStringLiteral("pushButton"));

        gridLayout->addWidget(pushButton, 6, 3, 1, 1);

        label = new QLabel(centralWidget);
        label->setObjectName(QStringLiteral("label"));

        gridLayout->addWidget(label, 0, 1, 1, 1);

        horizontalSpacer_2 = new QSpacerItem(40, 20, QSizePolicy::Expanding, QSizePolicy::Minimum);

        gridLayout->addItem(horizontalSpacer_2, 6, 2, 1, 1);

        ownerIdLineEdit = new QLineEdit(centralWidget);
        ownerIdLineEdit->setObjectName(QStringLiteral("ownerIdLineEdit"));

        gridLayout->addWidget(ownerIdLineEdit, 4, 2, 1, 1);

        facebookLineEdit = new QLineEdit(centralWidget);
        facebookLineEdit->setObjectName(QStringLiteral("facebookLineEdit"));

        gridLayout->addWidget(facebookLineEdit, 3, 2, 1, 1);

        wolframLineEdit = new QLineEdit(centralWidget);
        wolframLineEdit->setObjectName(QStringLiteral("wolframLineEdit"));

        gridLayout->addWidget(wolframLineEdit, 2, 2, 1, 1);

        databaseLineEdit = new QLineEdit(centralWidget);
        databaseLineEdit->setObjectName(QStringLiteral("databaseLineEdit"));

        gridLayout->addWidget(databaseLineEdit, 1, 2, 1, 1);

        tokenLineEdit = new QLineEdit(centralWidget);
        tokenLineEdit->setObjectName(QStringLiteral("tokenLineEdit"));

        gridLayout->addWidget(tokenLineEdit, 0, 2, 1, 1);

        horizontalSpacer = new QSpacerItem(40, 20, QSizePolicy::Maximum, QSizePolicy::Minimum);

        gridLayout->addItem(horizontalSpacer, 5, 3, 1, 1);


        gridLayout_2->addLayout(gridLayout, 0, 1, 1, 1);

        MyClassClass->setCentralWidget(centralWidget);
        menuBar = new QMenuBar(MyClassClass);
        menuBar->setObjectName(QStringLiteral("menuBar"));
        menuBar->setGeometry(QRect(0, 0, 400, 21));
        menuFile = new QMenu(menuBar);
        menuFile->setObjectName(QStringLiteral("menuFile"));
        MyClassClass->setMenuBar(menuBar);

        menuBar->addAction(menuFile->menuAction());
        menuFile->addAction(actionExit);

        retranslateUi(MyClassClass);
        QObject::connect(actionExit, SIGNAL(triggered()), MyClassClass, SLOT(close()));

        QMetaObject::connectSlotsByName(MyClassClass);
    } // setupUi

    void retranslateUi(QMainWindow *MyClassClass)
    {
        MyClassClass->setWindowTitle(QApplication::translate("MyClassClass", "Bot Config", Q_NULLPTR));
        actionExit->setText(QApplication::translate("MyClassClass", "Exit", Q_NULLPTR));
        label_3->setText(QApplication::translate("MyClassClass", "Wolfram Token:", Q_NULLPTR));
        label_5->setText(QApplication::translate("MyClassClass", "FacebookToken:", Q_NULLPTR));
        label_2->setText(QApplication::translate("MyClassClass", "Database Token:", Q_NULLPTR));
        label_4->setText(QApplication::translate("MyClassClass", "Owner ID:", Q_NULLPTR));
        pushButton->setText(QApplication::translate("MyClassClass", "Save to database", Q_NULLPTR));
        label->setText(QApplication::translate("MyClassClass", "Discord Token:", Q_NULLPTR));
        menuFile->setTitle(QApplication::translate("MyClassClass", "File", Q_NULLPTR));
    } // retranslateUi

};

namespace Ui {
    class MyClassClass: public Ui_MyClassClass {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_MYCLASS_H

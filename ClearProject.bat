@cls
@ECHO OFF

@rem Вывод нужных папок
rem dir _Resharper.* bin obj /s/b/a:d > listDirectories.txt
dir bin obj /s/b/a:d > listDirectories.txt

@rem Перечисление нужных папок
FOR /f "tokens=*" %%A IN (listDirectories.txt) DO @Call :MY_DEL %%A

del /F /S /Q *.user
del /F /S /Q *.xap
del /F /S /Q /A:H *.suo
del /F /S /Q *.suo
del /F /S /Q *.resharper
del /F /S /Q *.dxcache 
del listDirectories.txt
GoTo :EOF

@rem Удаление нужных папок  
:MY_DEL
IF NOT "%3"=="" (
  @echo Удаление %1 %2 %3
  rd /S /Q "%1 %2 %3"
  ) ELSE (
    IF NOT "%2"=="" (
      @echo Удаление %1 %2
      rd /S /Q "%1 %2"
      ) ELSE (
        @echo Удаление %1
        rd /S /Q "%1"
        )
  )
)


Goto :EOF


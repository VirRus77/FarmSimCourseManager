@cls
@ECHO OFF

@rem �뢮� �㦭�� �����
rem dir _Resharper.* bin obj /s/b/a:d > listDirectories.txt
dir bin obj /s/b/a:d > listDirectories.txt

@rem ����᫥��� �㦭�� �����
FOR /f "tokens=*" %%A IN (listDirectories.txt) DO @Call :MY_DEL %%A

del /F /S /Q *.user
del /F /S /Q *.xap
del /F /S /Q /A:H *.suo
del /F /S /Q *.suo
del /F /S /Q *.resharper
del /F /S /Q *.dxcache 
del listDirectories.txt
GoTo :EOF

@rem �������� �㦭�� �����  
:MY_DEL
IF NOT "%3"=="" (
  @echo �������� %1 %2 %3
  rd /S /Q "%1 %2 %3"
  ) ELSE (
    IF NOT "%2"=="" (
      @echo �������� %1 %2
      rd /S /Q "%1 %2"
      ) ELSE (
        @echo �������� %1
        rd /S /Q "%1"
        )
  )
)


Goto :EOF


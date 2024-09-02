# Processors Documentation

## Разница между ChainedWith() и EndChainWith()


### ChainedWith() возвращает то, что было передано в качестве аргумента


В случае

```
var Chain = ProcessorA
	    .ChainedWith(ProcessorB)
	    .ChainedWith(ProcessorC)  
```

__Chain__ будет указывать на __ProcessorC__, и при вызове __Chain.ProcessChainAsync()__, процессоры __ProcessorA__ и __ProcessorB__ не будут доступны.

Следующий код, в данном случае, делает одно и тоже:

```
Chain.ProcessChainAsync()
```

```
ProcessorC.ProcessChainAsync()
```


![alt text](ChainedWith.png "ChainedWith()")


### EndChainWith() возвращает то, что вызвало данный метод

В случае

```
var Chain = ProcessorA
	    .ChainedWith(ProcessorB)
	    .EndChainWith(ProcessorC)  
```

__Chain__ будет указывать на __ProcessorA__, и при вызове __Chain.ProcessChainAsync()__, процессоры ProcessorA, ProcessorB и ProcessorC выполнятся по цепочке в штатном режиме. 
Следующий код, в данном случае, делает одно и тоже:

```
Chain.ProcessChainAsync()
```

```
ProcessorA.ProcessChainAsync()
```

![alt text](EndChainWith.png "EndChainWith()")


## Пример создания цепочки

```
ProcessorA
.SubChainedWith(ProcessorB
    .ChainedWith(ProcessorC)
    .EndChainWith(ProcessorD))
.EndChainWith(ProcessorE)
.ProcessChainAsync()
```

Данный код делает тоже самое, что и код ниже:

```
var subchain = ProcessorB
             .ChainedWith(ProcessorC)
             .EndChainWith(ProcessorD)

var processorA_WithSubchain = ProcessorA
                              .SubChainedWith(subchain)

var Chain = processorA_WithSubchain
            .EndChainWith(ProcessorE)
            
Chain.ProcessChainAsync()          
```
На диаграмме это можно оттобразить следующим образом:

![alt text](ChainExample.png "ChainExample")

## Последовательность выполения __ProcessChainAsync()__

```
ProcessorA
.SubChainedWith(ProcessorB
    .ChainedWith(ProcessorC)
    .EndChainWith(ProcessorD))
.EndChainWith(ProcessorE)
.ProcessChainAsync()
```

### Приоритет __процессоров__:

ᐯ \
├──&nbsp; __1__ ProcessorA \
&nbsp;__|__&emsp;&emsp;├── __1.1__ ProcessorB \
&nbsp;__|__&emsp;&emsp;├── __1.2__ ProcessorC \
&nbsp;__|__&emsp;&emsp;└── __1.3__ ProcessorD \
&nbsp;__|__ \
└──&nbsp; __2__ ProcessorE


### Если ProcessorA НЕ выполнится, то он передаст управление ProcessorE.

#### Дочерняя цепочка ProcessorB ➞ ProcessorC ➞ ProcessorD будет проигнорирована!

ᐯ \
└──&nbsp; __2__ ProcessorE


### Если ProcessorA выполнится, то ProcessorE не будет вызван.

#### Далее ProcessorA передаст управление цепочке ProcessorB ➞ ProcessorC ➞ ProcessorD.

ᐯ \
└──&nbsp; __1__ ProcessorA &emsp;&emsp;&emsp;&emsp; ☑  \
&nbsp;&nbsp;&emsp;&emsp;├── __1.1__ ProcessorB \
&nbsp;&nbsp;&emsp;&emsp;├── __1.2__ ProcessorC \
&nbsp;&nbsp;&emsp;&emsp;└── __1.3__ ProcessorD 

### Предположим, что ProcessorB НЕ выполнился. Тогда он передаст управление в ProcessorC.

#### Предположим, что ProcessorC выполнился. Тогда он завершит цепочку, игнорируя ProcessorD.

ᐯ \
└──&nbsp; __1__ ProcessorA &emsp;&emsp;&emsp;&emsp; ☑  \
&nbsp;&nbsp;&emsp;&emsp;├── __1.1__ ProcessorB &emsp; ☒ \
&nbsp;&nbsp;&emsp;&emsp;└──  __1.2__ ProcessorC &emsp; ☑



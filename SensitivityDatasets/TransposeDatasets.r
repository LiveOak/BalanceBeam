rm(list=ls(all=TRUE))
directory <- "D:/Projects/OuHsc/BalanceBeam/SensitivityDatasets/"
#filenameInput <- "ChestPain.txt"
filenameInput <- "PediatricDyspnea.txt"
pathInput <- paste(directory, filenameInput, sep="")

dsInput <- read.table(pathInput, header=F, stringsAsFactors=F)
dsInput

featureLength <- nrow(dsInput)
dxLength <- ncol(dsInput)



for( rowIndex in 1:dxLength ) {
  str <- paste("((byte)", rowIndex, ", *DQ*", dsInput[1, rowIndex], "*DQ*", sep="") #After pasting output in Visual Studio, replace "*DQ*" with double quotes.
  for( columnIndex in 2:featureLength ) {
#    if( columnIndex > 1 ) str <- paste(str, ", ", sep="")
    str <- paste(str,  .01 * as.numeric(dsInput[columnIndex, rowIndex]),  sep=", ")
  }
  str <- paste(str, ");", sep="")
  print(str)
}

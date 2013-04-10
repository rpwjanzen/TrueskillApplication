odat <- read.table(text="info mean stddev color
Andrew 22.5960857723029 1.41700936900487 1
David 24.9754696002959 1.35660743072421 2
John 21.3629740097022 1.77821480238099 3
Kyle 23.4080468801531 1.34686613337352 4
Peter 26.67625465531 1.5664369865808 5
Prissy 26.7264506621461 1.3158724570531 6
Ryan 26.8777379850063 1.35178731565496 8", header=T)

dat <- transform(odat, lower = mean - 3 * stddev, upper = mean + 3 * stddev)

plot(
	x=c(min(dat$lower) - 2, max(dat$upper) + 2),
	y=c(0, .5),
	ylab="",
	xlim=c(min(dat$lower) - 2, max(dat$upper) + 2),
	xlab="",
	axes=FALSE,
	xaxs = "i",
	type="n")
legend("topright", legend=dat$info, lty=1, col=dat$color)
box()

FUN <- function(rownum) {
	par(new=TRUE)
	curve(
		dnorm(x, dat[rownum, 2], dat[rownum, 3]),
		xlim=c(c(min(dat$lower) - 2, max(dat$upper) + 2)), 
		ylim=c(0, .42),
		ylab="",
		xlab="",
		col=dat[rownum,4])
}

lapply(seq_len(nrow(dat)), function(i) FUN(i))
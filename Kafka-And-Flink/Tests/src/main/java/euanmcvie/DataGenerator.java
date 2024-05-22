package euanmcvie;

import java.util.Random;
import java.time.*;
import java.time.format.DateTimeFormatter;
import java.time.temporal.ChronoUnit;

public class DataGenerator {
    private Random random = new Random();

    public int getRandomInt() {
        return getRandomInt(Integer.MAX_VALUE);
    }

    public float getRandomFloat() {
        return getRandomFloat(Float.MAX_VALUE);
    }

    public String getNow() {
        return now(0);
    }

    private int getRandomInt(int max) {
        return random.nextInt(max);
    }

    private float getRandomFloat(float max) {
        return random.nextFloat() * max;
    }

    private String now(int additionalMilliseconds) {
        OffsetDateTime now = OffsetDateTime.now(ZoneOffset.UTC).plus(additionalMilliseconds, ChronoUnit.MILLIS);
        String nowString = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'").format(now);
        return nowString;
    }
}

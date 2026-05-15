import http from "k6/http";
import { check } from "k6";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5017";
const FLAG_KEY = __ENV.FLAG_KEY || "test-flag";
const ENVIRONMENT = __ENV.ENVIRONMENT || "Production";

const RATE = Number(__ENV.RATE || 1400);
const DURATION = __ENV.DURATION || "10m";
const PRE_ALLOCATED_VUS = Number(__ENV.PRE_ALLOCATED_VUS || 100);
const MAX_VUS = Number(__ENV.MAX_VUS || 1500);

const USER_IDS = [
    "11111111-1111-1111-1111-111111111111",
    "22222222-2222-2222-2222-222222222222",
    "33333333-3333-3333-3333-333333333333",
    "44444444-4444-4444-4444-444444444444",
    "55555555-5555-5555-5555-555555555555",
];

export const options = {
    scenarios: {
        soak: {
            executor: "constant-arrival-rate",
            rate: RATE,
            timeUnit: "1s",
            duration: DURATION,
            preAllocatedVUs: PRE_ALLOCATED_VUS,
            maxVUs: MAX_VUS,
        },
    },
    summaryTrendStats: ["avg", "min", "med", "p(90)", "p(95)", "p(99)", "max"],
    thresholds: {
        checks: ["rate>0.97"],
        http_req_failed: ["rate<0.03"],
        http_req_duration: ["p(95)<400"],
    },
};

export default function () {
    const userId = USER_IDS[Math.floor(Math.random() * USER_IDS.length)];
    const payload = JSON.stringify({
        flagKey: FLAG_KEY,
        environment: ENVIRONMENT,
        userId,
    });

    const res = http.post(`${BASE_URL}/api/evaluation`, payload, {
        headers: {
            "Content-Type": "application/json",
        },
        tags: {
            endpoint: "evaluation",
            profile: "soak",
        },
        timeout: __ENV.REQ_TIMEOUT || "5s",
    });

    check(res, {
        "status is 200": (r) => r.status === 200,
    });
}

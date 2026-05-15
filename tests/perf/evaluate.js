import http from "k6/http";
import { check } from "k6";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5017";
const FLAG_KEY = __ENV.FLAG_KEY || "test-flag";
const ENVIRONMENT = __ENV.ENVIRONMENT || "Production";

const USER_IDS = [
    "11111111-1111-1111-1111-111111111111",
    "22222222-2222-2222-2222-222222222222",
    "33333333-3333-3333-3333-333333333333",
    "44444444-4444-4444-4444-444444444444",
    "55555555-5555-5555-5555-555555555555",
];

export const options = {
    scenarios: {
        evaluation_load: {
            executor: "constant-arrival-rate",
            rate: 1400,
            timeUnit: "1s",
            duration: "30s",
            preAllocatedVUs: 50,
            maxVUs: 200,
        },
    },
    thresholds: {
        http_req_failed: ["rate<0.01"],
        http_req_duration: ["p(95)<300"],
        checks: ["rate>0.99"],
    },
};

export default function () {
    const userId = USER_IDS[Math.floor(Math.random() * USER_IDS.length)];

    const payload = JSON.stringify({
        flagKey: FLAG_KEY,
        environment: ENVIRONMENT,
        userId: userId,
    });

    const res = http.post(`${BASE_URL}/api/evaluation`, payload, {
        headers: {
            "Content-Type": "application/json",
        },
        tags: {
            endpoint: "evaluation",
        },
    });

    check(res, {
        "status is 200": (r) => r.status === 200,
    });
}
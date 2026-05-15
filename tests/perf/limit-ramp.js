import http from "k6/http";
import { check } from "k6";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5017";
const FLAG_KEY = __ENV.FLAG_KEY || "new_dashboard";
const ENVIRONMENT = __ENV.ENVIRONMENT || "Production";

const START_RATE = Number(__ENV.START_RATE || 1100);
const RATE_STEP = Number(__ENV.RATE_STEP || 50);
const STEPS = Number(__ENV.STEPS || 8);
const STEP_DURATION = __ENV.STEP_DURATION || "20s";
const PRE_ALLOCATED_VUS = Number(__ENV.PRE_ALLOCATED_VUS || 50);
const MAX_VUS = Number(__ENV.MAX_VUS || 500);

const USER_IDS = [
    "11111111-1111-1111-1111-111111111111",
    "22222222-2222-2222-2222-222222222222",
    "33333333-3333-3333-3333-333333333333",
    "44444444-4444-4444-4444-444444444444",
    "55555555-5555-5555-5555-555555555555",
];

function buildStages() {
    const stages = [];

    for (let i = 0; i < STEPS; i += 1) {
        stages.push({
            target: START_RATE + i * RATE_STEP,
            duration: STEP_DURATION,
        });
    }

    return stages;
}

export const options = {
    scenarios: {
        limit_ramp: {
            executor: "ramping-arrival-rate",
            startRate: START_RATE,
            timeUnit: "1s",
            preAllocatedVUs: PRE_ALLOCATED_VUS,
            maxVUs: MAX_VUS,
            stages: buildStages(),
        },
    },
    summaryTrendStats: ["avg", "min", "med", "p(90)", "p(95)", "p(99)", "max"],
    thresholds: {
        checks: ["rate>0.95"],
        http_req_failed: ["rate<0.05"],
        http_req_duration: ["p(95)<500"],
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
            profile: "limit-ramp",
        },
        timeout: __ENV.REQ_TIMEOUT || "5s",
    });

    check(res, {
        "status is 200": (r) => r.status === 200,
    });
}

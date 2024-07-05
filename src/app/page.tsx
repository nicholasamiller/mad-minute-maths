'use client'

import React, { useState, useEffect } from 'react'

function generateQuestion(): string {
  const max = 10
  const num1 = Math.floor(Math.random() * max) + 1
  const num2 = Math.floor(Math.random() * max) + 1
  const isAddition = Math.random() < 0.5

  if (isAddition) {
    return `${num1} + ${num2} = ____`
  } else {
    const largerNum = Math.max(num1, num2)
    const smallerNum = Math.min(num1, num2)
    return `${largerNum} - ${smallerNum} = ____`
  }
}

function generateQuestions(count: number): string[] {
  return Array(count).fill(null).map(() => generateQuestion())
}

export default function Home() {
  const [questions, setQuestions] = useState<string[]>([])

  useEffect(() => {
    setQuestions(generateQuestions(20))
  }, [])

  return (
    <div className="container mx-auto p-4 max-w-4xl">
      <h1 className="text-3xl font-bold mb-4 text-center">Millie&apos;s Mad Minute</h1>
      <p className="mb-4 text-center">Complete as many problems as you can in 1 minute. Go, Millie, go!</p>
      <div className="grid grid-cols-2 gap-4">
        {questions.map((question, index) => (
          <div key={index} className="border-2 border-gray-300 rounded p-4 text-center text-xl bg-white shadow">
            {question}
          </div>
        ))}
      </div>
      <div className="mt-8">
        <p>Date: _______________</p>
        <p>Time started: __________ Time finished: __________</p>
        <p>Score: ___ / 20</p>
      </div>
    </div>
  )
}